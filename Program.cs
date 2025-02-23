using CreekRiver.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region These three statements are important to have. Provided comments explaining each statement's purpose.
// The following statements makes an instance of the CreekRiverDbContext class available to our endpoints so they can 
// query the database.

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<CreekRiverDbContext>(builder.Configuration["CreekRiverDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

/*
 * ================================================ ENDPOINTS ================================================
 */

// The following endpoint is using LINQ methods that can be chained to db.Campsites. EF Core is turning this method chain
// into a SQL query (SELECT col1, col2,... FROM TableName) and then turning the tabular data that comes back from the db
// into .NET objects.
// ASP.NET is serializing (the process of converting .NET objects into a format that can be transmitted
// and understood by other systems) those .NET objects into JSON to send back to the client.
// Also, the endpoint provides access to the DbContext for our db by adding another param to the handler. This is a basic form
// of dependency injection, where EF Core sees a dependency that the handler requires, and passes in an instance of it as an 
// arg so that the handler can use it.

#region GET Endpoints for /api/campsite
app.MapGet("/api/campsites", (CreekRiverDbContext db) => db.Campsites.ToList());
app.MapGet("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
{
    // Include adds a SQL JOIN in the underlying SQL query to the CampsiteTypes table.
    var campsite = db.Campsites.Include(c => c.CampsiteType).SingleOrDefault(c => c.Id == id);
    if (campsite == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(campsite);
});
#endregion

#region POST and PUT Endpoints for /api/campsite
app.MapPost("/api/campsites", (CreekRiverDbContext db, Campsite campsite) =>
{
    db.Campsites.Add(campsite);
    db.SaveChanges(); // Inherited in CreekRiverDbContext from the DbContext class. This sends the change to the DB.
    return Results.Created($"/api/campsite/{campsite.Id}", campsite); // Method creates a 201 response (resource created).
});

app.MapPut("/api/campsites/{id}", (CreekRiverDbContext db, int id, Campsite campsite) =>
{
    Campsite campsitePayload = db.Campsites.SingleOrDefault(c => c.Id == id);
    if (campsitePayload == null)
    {
        return Results.NotFound();
    }
    campsitePayload.Nickname = campsite.Nickname;
    campsitePayload.CampsiteTypeId = campsite.CampsiteTypeId;
    campsitePayload.ImageUrl = campsite.ImageUrl;

    db.SaveChanges();
    return Results.NoContent(); // Success message is all that is needed when returning a response back to the client. 
});
#endregion

#region All DELETE endpoints
app.MapDelete("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
{
    Campsite campsite = db.Campsites.SingleOrDefault(c => c.Id == id);
    if (campsite == null)
    {
        return Results.NotFound();
    }
    db.Campsites.Remove(campsite);
    db.SaveChanges();
    return Results.NoContent();
});
#endregion

// Getting Reservations with Related Data
app.MapGet("/api/reservations", (CreekRiverDbContext db) =>
{
    // Each of these method chains can be thought of as an SQL query.
    return db.Reservations
        .Include(r => r.UserProfile) // This will JOIN the UserProfiles table.
        .Include(r => r.Campsite) // Will further JOIN the Campsite table.
        // Use .ThenInclude when you want to JOIN to a table that is not the original table meaning, in this scenario,
        // you can only get to CampsiteType from Campsites, not the original Reservations table. We're adding CampsiteTypes
        // data to the Campsites data right after the Include call to JOIN campsites.
        .ThenInclude(c => c.CampsiteType) // Goes into Campsite to get the related CampsiteType.
        .OrderBy(res => res.CheckinDate)
        .ToList();
    // Try the endpoint out. You will notice that if a campsite has reservations associated with it, EF Core will also
    // populate the reservation data for those campsites nested in the reservations property. This is ok, and is called
    // "navigation property fix-up". For the most part, you can ignore this extra data, but occasionally you might find
    // it useful.
});

app.MapPost("/api/reservations", (CreekRiverDbContext db, Reservation reservationPayload) =>
{
    try
    {
        if (reservationPayload.CheckinDate < DateTime.Now)
        {
            return Results.BadRequest("Please check in a future date.");
        }
        if (reservationPayload.CheckoutDate < DateTime.Now)
        {
            return Results.BadRequest("Please check in a future date.");
        }
        db.Reservations.Add(reservationPayload);
        db.SaveChanges();
        return Results.Created($"/api/reservations/{reservationPayload.Id}", reservationPayload);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid data submitted.");
    }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
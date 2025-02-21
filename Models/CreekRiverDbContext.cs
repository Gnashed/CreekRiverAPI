// This class file is able to access the PostgreSQL database and seed the database with data.

using Microsoft.EntityFrameworkCore;
using CreekRiver.Models;

namespace CreekRiver.Models;

// DBContext is a class that comes from EF Core that represents our database as .NET objects that we can access.
// Here we are using inheritance which allows the CreekRiverDbContext class to inherit all the properties, fields, and
// methods of another class. Inheritance indicates an "is-a" relationship b/w two types. All properties of DbContext 
// allow this class to connect to the database with no additional code needed.
public class CreekRiverDbContext : DbContext
{
    // The following properties of collections for the CreekRiverDbContext class corresponds to the tables in our DB. By
    // adding it to this class, EF Core will know which classes represent our database entities.
    // Adding DbSet is another class from EF Core, which is like other collections (list, Array), in that we can write 
    // LINQ queries to get data from them. The LINQ queries will be transformed into a SQL query which will then run
    // against the database.
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Campsite> Campsites { get; set; }
    public DbSet<CampsiteType> CampsiteTypes { get; set; }
    
    // Here is a constructor (a method-like member of a class that constructs an instance of an object).
    // The base keyword passes the options object down to DbContext when ASP.NET creates CreekRiverDbContext class.
    public CreekRiverDbContext(DbContextOptions<CreekRiverDbContext> context) : base(context)
    {
        
    }
    
    // The access modifier `protected` means the method can only be called from code inside the class itself or by a 
    // class that inherits it. This is a form of the OOP concept, encapsulation, which restricts direct access to some
    // of the object's components. This prevents external code from being concerned with the internal workings of an obj.
    // override indicates that OnModelCreating is replacing a method of the same name that is inherited from the DbContext
    // class.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data with campsite types
        modelBuilder.Entity<CampsiteType>().HasData(new CampsiteType[]
        {
            new CampsiteType
            {
                Id = 1,
                CampsiteTypeName = "Tent",
                FeePerNight = 15.99M,
                MaxReservationDays = 7
            },
            new CampsiteType
            {
                Id = 2,
                CampsiteTypeName = "RV",
                FeePerNight = 26.50M,
                MaxReservationDays = 14
            },
            new CampsiteType
            {
                Id = 3,
                CampsiteTypeName = "Primitive",
                FeePerNight = 10.00M,
                MaxReservationDays = 3
            },
            new CampsiteType
            {
                Id = 4,
                CampsiteTypeName = "Hammock",
                FeePerNight = 12M,
                MaxReservationDays = 7
            }
        });

        modelBuilder.Entity<Campsite>().HasData(new Campsite[]
        {
            new Campsite
            {
                Id = 1,
                CampsiteTypeId = 1,
                Nickname = "Barred Owl",
                ImageUrl = "https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg"
            },
            new Campsite
            {
                Id = 2,
                CampsiteTypeId = 2,
                Nickname = "Big Hill Pond",
                ImageUrl = "https://tnstateparks.com/assets/images/content-images/social-media-images/big-hill-pond.jpg"
            },
            new Campsite
            {
                Id = 3,
                CampsiteTypeId = 1,
                Nickname = "Cumberland Mountain",
                ImageUrl = "https://tnstateparks.com/assets/images/content-images/blog/4270/ctpp_-_peter_koczera_-_black_mountain_overlook.jpg"
            },
            new Campsite
            {
                Id = 4,
                CampsiteTypeId = 3,
                Nickname = "Long Hunter",
                ImageUrl = "https://nashvillefunforfamilies.com/wp-content/uploads/2016/06/Long-Hunter-State-Park.jpg"
            },
            new Campsite
            {
                Id = 5,
                CampsiteTypeId = 2,
                Nickname = "Natchez Trace",
                ImageUrl = "https://tnstateparks.com/assets/images/content-images/campgrounds/5030/natchez-trace_camping-cabin1.jpg"
            },
            new Campsite
            {
                Id = 6,
                CampsiteTypeId = 3,
                Nickname = "Frozen Head",
                ImageUrl = "https://i.redd.it/z7xea2cu2s451.jpg"
            },
            new Campsite
            {
                Id = 7,
                CampsiteTypeId = 4,
                Nickname = "Warriors’ Path",
                ImageUrl = "https://tnstateparks.com/assets/images/content-images/campgrounds/279/wpsp-a5-4.jpg"
            }
        });
    }
}
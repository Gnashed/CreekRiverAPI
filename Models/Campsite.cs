using System.ComponentModel.DataAnnotations;

namespace CreekRiver.Models;

public class Campsite
{
    public int Id { get; set; }
    [Required] // Makes the variable below `NOT NULLABLE` in the SQL table.
    public string Nickname { get; set; }
    public string ImageUrl { get; set; }
    public int CampsiteTypeId { get; set; } // EF Core can infer that this is a foreign key to CampsiteType.
    public CampsiteType CampsiteType { get; set; }
    public List<Reservation> Reservations { get; set; } // Stores reservations associated with a particular campsite.
}
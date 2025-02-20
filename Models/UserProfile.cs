using System.ComponentModel.DataAnnotations;

namespace CreekRiver.Models;

public class UserProfile
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    
    // UserProfile can be associated with multiple Reservation records in a one-to-many relationship.
    public List<Reservation> Reservations { get; set; }
}
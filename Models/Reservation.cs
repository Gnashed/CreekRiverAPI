namespace CreekRiver.Models;

public class Reservation
{
    public int Id { get; set; }
    public int CampsiteId { get; set; } // FK
    public Campsite Campsite { get; set; }
    public int UserProfileId { get; set; } // FK
    // UserProfile is ignored by EF Core when creating the UserProfile table. This is so that we can use it to 
    // store the UserProfile data from the UserProfile table that corresponds to UserProfileId.
    public UserProfile UserProfile { get; set; }
    public DateTime CheckinDate { get; set; }
    public DateTime CheckoutDate { get; set; }
}
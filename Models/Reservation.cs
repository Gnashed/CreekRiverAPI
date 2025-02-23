namespace CreekRiver.Models;

public class Reservation
{
    public int Id { get; set; }
    public int CampsiteId { get; set; } // FK
    public Campsite Campsite { get; set; } // Ignored by EF Core
    public int UserProfileId { get; set; } // FK
    // UserProfile is ignored by EF Core when creating the UserProfile table. This is so that we can use it to 
    // store the UserProfile data from the UserProfile table that corresponds to UserProfileId.
    public UserProfile UserProfile { get; set; }
    public DateTime CheckinDate { get; set; }
    public DateTime CheckoutDate { get; set; }
    // TotalNights won't correspond to a column in the DB. ASP.NET will evaluate the calculated properties and add 
    // them to the JSON. This is also equivalent to defining a getter, meaning this property allows it to be read-only,
    // which is great for when doing calculations.
    public int TotalNights => (CheckoutDate - CheckinDate).Days;
    // This class member is called a field. get and set are omitted since this is not a property, but a member.
    // private is another access modifier that prevents this field from being accessed outside the code that is contained
    // in the class. It will be ignored in the JSON and won't appear in the HTTP responses.
    // static means that the value for this field will be shared across all instances of Reservation.
    // By convention, private fields are prepended with an underscore (_).
    private static readonly decimal _reservationBaseFee = 10M;
    public decimal? TotalCost
    {
        get
        {
            if (Campsite?.CampsiteType != null)
            {
                return Campsite.CampsiteType.FeePerNight * TotalNights + _reservationBaseFee;
            }
            return null;
        }
    }
}
namespace Travellers.Trips.Hotels;

public class TripHotelReservationRow
{
    public Guid TripHotelReservationId { get; set; }
    public Guid TripId { get; set; }
    public string HubReservationId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

namespace Travellers.Trips;

public class TripRow
{
    public Guid TripId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

namespace Travellers.Trips.Hotels;

public interface IHotelHubClient
{
    Task<HotelReservation> GetReservationAsync(string hubReservationId, CancellationToken ct);
}

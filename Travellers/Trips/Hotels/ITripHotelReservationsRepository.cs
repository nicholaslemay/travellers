namespace Travellers.Trips.Hotels;

public interface ITripHotelReservationsRepository
{
    Task<IReadOnlyList<string>> GetHubReservationIdsAsync(Guid tripId, CancellationToken ct);
}

namespace Travellers.Trips.Hotels;

public class GetHotelReservationsForTripUseCase(
    ITripHotelReservationsRepository repository,
    IHotelHubClient hotelHubClient)
{
    public async Task<IReadOnlyList<HotelReservationResponse>> ExecuteAsync(Guid tripId, CancellationToken ct)
    {
        var hubReservationIds = await repository.GetHubReservationIdsAsync(tripId, ct);

        var reservations = await Task.WhenAll(
            hubReservationIds.Select(id => hotelHubClient.GetReservationAsync(id, ct)));

        return reservations
            .Select(r => new HotelReservationResponse(r.HotelName, r.CheckIn, r.CheckOut))
            .ToList();
    }
}

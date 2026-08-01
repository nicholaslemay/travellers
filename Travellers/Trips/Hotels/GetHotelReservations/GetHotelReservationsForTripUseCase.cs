using OneOf;
using Travellers.Support;

namespace Travellers.Trips.Hotels.GetHotelReservations;

public class GetHotelReservationsForTripUseCase(
    ITripRepository repository,
    IHotelHubClient hotelHubClient)
{
    public async Task<OneOf<List<HotelReservation>, NotFound>> ExecuteAsync(TripId tripId, CancellationToken ct)
    {
        var result = await repository.GetTripAsync(tripId, ct);

        return await result.Match<Task<OneOf<List<HotelReservation>, NotFound>>>(
            async trip => await FetchReservationsAsync(trip, ct),
            notFound => Task.FromResult<OneOf<List<HotelReservation>, NotFound>>(notFound)
        );
    }

    private async Task<List<HotelReservation>> FetchReservationsAsync(Trip trip, CancellationToken ct)
    {
        var reservations = await Task.WhenAll(
            trip.HotelReservations.Select(r => hotelHubClient.GetReservationAsync(r.HubReservationId, ct)));

        return reservations.ToList();
    }
}

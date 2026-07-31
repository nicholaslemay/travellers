namespace Travellers.Trips.Hotels;

public class GetHotelReservationsForTripUseCase
{
    public Task<IReadOnlyList<HotelReservationResponse>> ExecuteAsync(Guid tripId, CancellationToken ct)
    {
        IReadOnlyList<HotelReservationResponse> reservations =
        [
            new HotelReservationResponse(
                HotelName: "Le Méridien Paris",
                CheckIn: new DateOnly(2026, 8, 15),
                CheckOut: new DateOnly(2026, 8, 18)
            )
        ];

        return Task.FromResult(reservations);
    }
}

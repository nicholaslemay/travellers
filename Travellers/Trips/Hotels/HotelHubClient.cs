namespace Travellers.Trips.Hotels;

public class HotelHubClient : IHotelHubClient
{
    public Task<HotelReservation> GetReservationAsync(string hubReservationId, CancellationToken ct) =>
        Task.FromResult(new HotelReservation(
            HotelName: "Le Méridien Paris",
            CheckIn: new DateOnly(2026, 8, 15),
            CheckOut: new DateOnly(2026, 8, 18)
        ));
}

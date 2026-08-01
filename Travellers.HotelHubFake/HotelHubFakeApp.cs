namespace Travellers.HotelHubFake;

internal record FakeReservation(string HotelName, DateOnly CheckInDate, DateOnly CheckOutDate);

public class FakeReservationStore
{
    private readonly Dictionary<string, FakeReservation> _reservations = new();

    internal void Add(string reservationId, FakeReservation reservation) =>
        _reservations[reservationId] = reservation;

    internal FakeReservation? Get(string reservationId) =>
        _reservations.GetValueOrDefault(reservationId);
}

public static class HotelHubFakeApp
{
    public static void MapEndpoints(WebApplication app, FakeReservationStore store)
    {
        app.MapGet("/v1/reservations/{reservationId}", (string reservationId) =>
        {
            var reservation = store.Get(reservationId);
            if (reservation is null) return Results.NotFound();
            return Results.Ok(new
            {
                reservationId,
                hotel = new { name = reservation.HotelName },
                checkInDate = reservation.CheckInDate,
                checkOutDate = reservation.CheckOutDate
            });
        });
    }
}

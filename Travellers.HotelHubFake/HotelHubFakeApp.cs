namespace Travellers.HotelHubFake;

public static class HotelHubFakeApp
{
    public static void MapEndpoints(WebApplication app)
    {
        app.MapGet("/v1/reservations/{reservationId}", (string reservationId) =>
            Results.Ok(new
            {
                reservationId,
                hotel = new
                {
                    name = "Le Méridien Paris",
                    address = new { city = "Paris", countryCode = "FR" }
                },
                checkInDate = "2026-08-15",
                checkOutDate = "2026-08-18"
            }));
    }
}

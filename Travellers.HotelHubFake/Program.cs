var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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

app.Run();

public partial class Program { }

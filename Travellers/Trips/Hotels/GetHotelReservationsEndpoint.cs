namespace Travellers.Trips.Hotels;

public static class GetHotelReservationsEndpoint
{
    public static IEndpointRouteBuilder MapGetHotelReservationsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/trips/{tripId}/reservations/hotels", (Guid tripId) =>
            Results.Ok(new GetHotelReservationsResponse(
                TripId: tripId,
                Hotels:
                [
                    new HotelReservationResponse(
                        HotelName: "Le Méridien Paris",
                        CheckIn: new DateOnly(2026, 8, 15),
                        CheckOut: new DateOnly(2026, 8, 18)
                    )
                ]
            )));

        return app;
    }
}

namespace Travellers.Trips.Hotels;

public static class GetHotelReservationsEndpoint
{
    public static IEndpointRouteBuilder MapGetHotelReservationsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/trips/{tripId}/reservations/hotels", async (
            Guid tripId,
            GetHotelReservationsForTripUseCase useCase,
            CancellationToken ct) =>
        {
            var hotels = await useCase.ExecuteAsync(tripId, ct);
            return Results.Ok(new GetHotelReservationsResponse(TripId: tripId, Hotels: hotels));
        });

        return app;
    }
}

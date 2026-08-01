using OneOf;
using Travellers.Support;

namespace Travellers.Trips.Hotels.GetHotelReservations;

public static class GetHotelReservationsEndpoint
{
    public static IEndpointRouteBuilder MapGetHotelReservationsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/trips/{tripId}/reservations/hotels", async (
            Guid tripId,
            GetHotelReservationsForTripUseCase useCase,
            CancellationToken ct) =>
        {
            var result = await useCase.ExecuteAsync(new TripId(tripId), ct);

            return result.Match(
                hotels => Results.Ok(new GetHotelReservationsResponse(tripId, hotels
                    .Select(h => new HotelReservationResponse(h.HotelName, h.CheckIn, h.CheckOut))
                    .ToList())),
                _ => Results.NotFound()
            );
        });

        return app;
    }
}

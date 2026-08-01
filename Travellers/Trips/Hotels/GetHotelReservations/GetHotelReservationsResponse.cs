namespace Travellers.Trips.Hotels.GetHotelReservations;

public record GetHotelReservationsResponse(
    Guid TripId,
    IReadOnlyList<HotelReservationResponse> Hotels
);

public record HotelReservationResponse(
    string HotelName,
    DateOnly CheckIn,
    DateOnly CheckOut
);

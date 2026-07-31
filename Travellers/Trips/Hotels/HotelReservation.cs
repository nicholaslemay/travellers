namespace Travellers.Trips.Hotels;

public record HotelReservation(
    string HotelName,
    DateOnly CheckIn,
    DateOnly CheckOut
);

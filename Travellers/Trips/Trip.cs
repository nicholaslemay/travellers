namespace Travellers.Trips;

public record TripId(Guid Value);

public record TripHotelReservation(string HubReservationId);

public record Trip(TripId Id, IReadOnlyList<TripHotelReservation> HotelReservations);

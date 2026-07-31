namespace Travellers.Trips.Hotels;

public static class HotelReservationsModuleConfiguration
{
    public static IServiceCollection AddHotelReservationsModule(this IServiceCollection services) =>
        services
            .AddScoped<IHotelHubClient, HotelHubClient>()
            .AddScoped<ITripHotelReservationsRepository, TripHotelReservationsRepository>()
            .AddScoped<GetHotelReservationsForTripUseCase>();
}

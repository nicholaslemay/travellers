namespace Travellers.Trips.Hotels;

public static class HotelReservationsModuleConfiguration
{
    public static IServiceCollection AddHotelReservationsModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<IHotelHubClient, HotelHubClient>(client =>
            client.BaseAddress = new Uri(config["HotelHub:BaseUrl"]!));

        return services
            .AddScoped<ITripHotelReservationsRepository, TripHotelReservationsRepository>()
            .AddScoped<GetHotelReservationsForTripUseCase>();
    }
}

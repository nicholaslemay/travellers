namespace Travellers.Users;

public static class UsersModuleConfiguration
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UsersRepository>();

        return services;
    }
}

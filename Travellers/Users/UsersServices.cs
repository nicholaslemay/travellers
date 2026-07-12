using Microsoft.Extensions.DependencyInjection;

namespace Travellers.Users;

public static class UsersServices
{
    public static IServiceCollection AddUsers(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UsersRepository>();

        return services;
    }
}

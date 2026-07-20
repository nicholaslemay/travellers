using FluentValidation;
using Travellers.Support.Validation;

namespace Travellers.Users;

public static class UsersModuleConfiguration
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UsersRepository>();
        services.AddScoped<CreateUserUseCase>();

        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        services.AddScoped<IRequestValidator<CreateUserRequest>, FluentValidationRequestValidator<CreateUserRequest>>();

        return services;
    }
}

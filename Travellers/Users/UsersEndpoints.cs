using Travellers.Support.Validation;

namespace Travellers.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (
            CreateUserRequest request,
            IRequestValidator<CreateUserRequest> validator,
            CreateUserUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var validation = validator.Validate(request);

            if (!validation.IsValid)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>(validation.Errors));
            }

            var user = await useCase.ExecuteAsync(request.Email, cancellationToken);

            return Results.Created($"/users/{user.Id.Value}", user);
        });

        return app;
    }
}

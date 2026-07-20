using Travellers.Support.Validation;

namespace Travellers.Users.Create;

public static class CreateUserEndpoint
{
    public static IEndpointRouteBuilder MapCreateUserEndpoint(this IEndpointRouteBuilder app)
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

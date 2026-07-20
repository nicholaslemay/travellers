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

            var result = await useCase.ExecuteAsync(request.Email, cancellationToken);

            if (result.EmailWasAlreadyTaken)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Email already exists",
                    detail: $"A user with email '{request.Email}' already exists.");
            }

            return Results.Created($"/users/{result.User!.Id.Value}", result.User);
        });

        return app;
    }
}

using Travellers.Users;

namespace Travellers.Users.Create;

public class CreateUserUseCase(IUserRepository repository)
{
    public async Task<CreateUserResult> ExecuteAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailAlreadyExists = await repository
            .ExistsByEmailAsync(email, cancellationToken)
            .ConfigureAwait(false);

        if (emailAlreadyExists)
        {
            return CreateUserResult.EmailAlreadyExists();
        }

        var user = await repository.CreateUserAsync(email, cancellationToken).ConfigureAwait(false);

        return CreateUserResult.Created(user);
    }
}

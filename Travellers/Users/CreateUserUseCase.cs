namespace Travellers.Users;

public class CreateUserUseCase(IUserRepository repository)
{
    public Task<User> ExecuteAsync(string email, CancellationToken cancellationToken = default) =>
        repository.CreateUserAsync(email, cancellationToken);
}

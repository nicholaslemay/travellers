namespace Travellers.Users;

public interface IUserRepository
{
    Task<User> CreateUserAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}

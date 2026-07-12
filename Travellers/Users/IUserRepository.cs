namespace Travellers.Users;

public interface IUserRepository
{
    User CreateUser(string email);

    User? GetById(UserId id);

    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
}

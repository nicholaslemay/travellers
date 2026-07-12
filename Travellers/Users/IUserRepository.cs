namespace Travellers.Users;

public interface IUserRepository
{
    User CreateUser(string email);

    User? GetById(UserId id);
}

using Travellers.Support.Db;

namespace Travellers.Users;

public class UsersRepository(TravellersDbContext dbContext, TimeProvider timeProvider) : IUserRepository
{
    public User CreateUser(string email)
    {
        var timestamp = timeProvider.GetUtcNow();

        var row = new UserRow
        {
            Email = email,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };

        dbContext.Set<UserRow>().Add(row);
        dbContext.SaveChanges();

        return BuildUserFrom(row);
    }

    public User? GetById(UserId id)
    {
        var row = dbContext.Set<UserRow>().Find(id.Value);

        return row is null ? null : BuildUserFrom(row);
    }

    private static User BuildUserFrom(UserRow row) =>
        new(new UserId(row.UserId), row.Email, row.CreatedAt, row.UpdatedAt);
}

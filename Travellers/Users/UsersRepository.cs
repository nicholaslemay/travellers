using Microsoft.EntityFrameworkCore;
using Travellers.Support.Db;

namespace Travellers.Users;

public class UsersRepository(
    TravellersDbContext dbContext,
    TimeProvider timeProvider,
    DatabaseExecutor database) : IUserRepository
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

    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default) =>
        database.ExecuteAsync(async token =>
        {
            var row = await dbContext.Set<UserRow>()
                .FirstOrDefaultAsync(user => user.UserId == id.Value, token)
                .ConfigureAwait(false);

            return row is null ? null : BuildUserFrom(row);
        }, cancellationToken);

    private static User BuildUserFrom(UserRow row) =>
        new(new UserId(row.UserId), row.Email, row.CreatedAt, row.UpdatedAt);
}

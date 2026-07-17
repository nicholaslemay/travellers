using Microsoft.EntityFrameworkCore;
using Travellers.Support.Db;

namespace Travellers.Users;

public class UsersRepository(TimeProvider timeProvider, DatabaseExecutor database) : IUserRepository
{
    public Task<User> CreateUserAsync(string email, CancellationToken cancellationToken = default) =>
        database.ExecuteAsync(async (context, token) =>
        {
            var timestamp = timeProvider.GetUtcNow();

            var row = new UserRow
            {
                Email = email,
                CreatedAt = timestamp,
                UpdatedAt = timestamp
            };

            context.Set<UserRow>().Add(row);
            await context.SaveChangesAsync(token).ConfigureAwait(false);

            return BuildUserFrom(row);
        }, cancellationToken);

    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default) =>
        database.ExecuteAsync(async (context, token) =>
        {
            var row = await context.Set<UserRow>()
                .FirstOrDefaultAsync(user => user.UserId == id.Value, token)
                .ConfigureAwait(false);

            return row is null ? null : BuildUserFrom(row);
        }, cancellationToken);

    private static User BuildUserFrom(UserRow row) =>
        new(new UserId(row.UserId), row.Email, row.CreatedAt, row.UpdatedAt);
}

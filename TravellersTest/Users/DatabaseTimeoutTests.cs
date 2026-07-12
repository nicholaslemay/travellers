using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Travellers.Support.Db;
using Travellers.Users;

namespace TravellersTest.Users;

[Collection("Database")]
public class DatabaseTimeoutTests(DatabaseMigrationFixture fixture)
{
    private static readonly TimeSpan ConfiguredTimeout = TimeSpan.FromSeconds(90);

    [Fact(Timeout = 5000)]
    public async Task ShouldTimeOutWhenDatabaseCallExceedsConfiguredTimeout()
    {
        var fakeTime = new FakeTimeProvider();
        var options = new DatabaseResilienceOptions { Timeout = ConfiguredTimeout };
        var pipeline = DatabaseResiliencePipeline.Build(options, fakeTime);

        var interceptor = new HangingCommandInterceptor();
        await using var connection = new SqlConnection(fixture.ConnectionString);
        await connection.OpenAsync();
        var contextOptions = new DbContextOptionsBuilder<TravellersDbContext>()
            .UseSqlServer(connection)
            .AddInterceptors(interceptor)
            .Options;
        await using var dbContext = new TravellersDbContext(contextOptions);

        var repository = new UsersRepository(dbContext, fakeTime, new DatabaseExecutor(pipeline));

        var call = repository.GetByIdAsync(new UserId(1));

        await interceptor.CommandStarted.ConfigureAwait(true);
        fakeTime.Advance(options.Timeout + TimeSpan.FromTicks(1));

        var completingTheCall = async () => await call.ConfigureAwait(true);

        await completingTheCall.Should().ThrowAsync<DatabaseTimeoutException>();
    }
}

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Travellers.Support.Db;

namespace TravellersTest.Support.Db;

[Collection("Database")]
public class DatabaseExecutorTests(DatabaseMigrationFixture fixture) : DatabaseTest(fixture)
{
    [Fact(Timeout = 5000)]
    public async Task ShouldTimeOutOnHangingReadOperation()
    {
        var executor = GetService<DatabaseExecutor>();
        var configuredTimeout = GetService<IOptions<DatabaseResilienceOptions>>().Value.Timeout;

        SetupDatabaseToHang();
        var call = executor.ExecuteAsync((context, token) =>
            context.Database.SqlQueryRaw<int>("SELECT 1").ToListAsync(token));

        FakeTime.Advance(configuredTimeout + TimeSpan.FromTicks(1));

        var completingTheCall = async () => await call.ConfigureAwait(true);

        await completingTheCall.Should().ThrowAsync<DatabaseTimeoutException>();
    }

    [Fact(Timeout = 5000)]
    public async Task ShouldTimeOutOnHangingWriteOperation()
    {
        var executor = GetService<DatabaseExecutor>();
        var configuredTimeout = GetService<IOptions<DatabaseResilienceOptions>>().Value.Timeout;

        SetupDatabaseToHang();
        var call = executor.ExecuteAsync((context, token) =>
            context.Database.ExecuteSqlRawAsync("SELECT 1", token));

        FakeTime.Advance(configuredTimeout + TimeSpan.FromTicks(1));

        var completingTheCall = async () => await call.ConfigureAwait(true);

        await completingTheCall.Should().ThrowAsync<DatabaseTimeoutException>();
    }
}

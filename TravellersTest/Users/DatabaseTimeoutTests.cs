using FluentAssertions;
using Microsoft.Extensions.Options;
using Travellers.Support.Db;
using Travellers.Users;
using TravellersTest.Support;

namespace TravellersTest.Users;

[Collection("Database")]
public class DatabaseTimeoutTests(DatabaseMigrationFixture fixture) : DatabaseTest(fixture)
{
    [Fact(Timeout = 5000)]
    public async Task ShouldTimeOutWhenDatabaseCallExceedsConfiguredTimeout()
    {
        SetupDatabaseToHang();

        var repository = GetService<IUserRepository>();
        var configuredTimeout = GetService<IOptions<DatabaseResilienceOptions>>().Value.Timeout;

        var call = repository.GetByIdAsync(new UserId(1));

        FakeTime.Advance(configuredTimeout + TimeSpan.FromTicks(1));

        var completingTheCall = async () => await call.ConfigureAwait(true);

        await completingTheCall.Should().ThrowAsync<DatabaseTimeoutException>();
    }
}

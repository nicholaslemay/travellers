using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Travellers.Support.Db;
using Travellers.Users;
using TravellersTest.Support;

namespace TravellersTest.Users;

[Collection("Database")]
public class DatabaseTimeoutTests
{
    [Fact(Timeout = 5000)]
    public async Task ShouldTimeOutWhenDatabaseCallExceedsConfiguredTimeout()
    {
        var fakeTime = new FakeTimeProvider();
        var interceptor = new HangingCommandInterceptor();

        await using var factory = new TravellersWebApplicationFactory()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(fakeTime);
                services.AddSingleton<IInterceptor>(interceptor);
            }));

        using var scope = factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var repository = serviceProvider.GetRequiredService<IUserRepository>();
        var configuredTimeout = serviceProvider
            .GetRequiredService<IOptions<DatabaseResilienceOptions>>().Value.Timeout;

        var call = repository.GetByIdAsync(new UserId(1));

        await interceptor.CommandStarted.ConfigureAwait(true);
        fakeTime.Advance(configuredTimeout + TimeSpan.FromTicks(1));

        var completingTheCall = async () => await call.ConfigureAwait(true);

        await completingTheCall.Should().ThrowAsync<DatabaseTimeoutException>();
    }
}

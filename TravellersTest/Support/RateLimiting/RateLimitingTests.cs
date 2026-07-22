using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Travellers.Support.Extensions;
using Travellers.Support.RateLimiting;
using Travellers.Users.Create;

namespace TravellersTest.Support.RateLimiting;

[Collection("Database")]
public class RateLimitingTests : DatabaseTest
{
    public RateLimitingTests(DatabaseMigrationFixture fixture) : base(fixture)
    {
        OverrideServices(services => services.Configure<RateLimitingOptions>(options =>
        {
            options.PermitLimit = 3;
            options.Window = TimeSpan.FromSeconds(10);
        }));
    }

    [Fact]
    public async Task ShouldAllowRequestsUpToConfiguredLimit()
    {
        var client = CreateHttpClient();
        var maximumNumberOfRequests = GetService<IOptions<RateLimitingOptions>>().Value.PermitLimit;

        await maximumNumberOfRequests.Times(async () =>
        {
            var response = await client.GetAsync("/");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        });

    }

    [Fact]
    public async Task ShouldRejectRequestExceedingConfiguredLimitWithRetryAfterHeader()
    {
        var client = CreateHttpClient();
        var options = GetService<IOptions<RateLimitingOptions>>().Value;

        var maximumNumberOfRequests = GetService<IOptions<RateLimitingOptions>>().Value.PermitLimit;
        
        await (maximumNumberOfRequests + 1).Times(async () =>
            await client.GetAsync("/")
        );

        var response = await client.GetAsync("/");

        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        response.Headers.RetryAfter.Should().NotBeNull();
        response.Headers.RetryAfter!.Delta.Should().Be(options.Window);
    }

    [Fact]
    public async Task ShouldApplyLimitGloballyAcrossDifferentEndpoints()
    {
        var client = CreateHttpClient();
        var maximumNumberOfRequests = GetService<IOptions<RateLimitingOptions>>().Value.PermitLimit;

        await (maximumNumberOfRequests + 1).Times(async () =>
            await client.GetAsync("/")
        );

        var response = await client.PostAsJsonAsync("/users", new CreateUserRequest("traveller@example.com"));

        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task ShouldAllowRequestsAgainAfterWindowElapses()
    {
        OverrideServices(services => services.Configure<RateLimitingOptions>(options =>
        {
            options.Window = TimeSpan.FromMilliseconds(200);
        }));

        var client = CreateHttpClient();
        var options = GetService<IOptions<RateLimitingOptions>>().Value;

        var maximumNumberOfRequests = options.PermitLimit;

        await (maximumNumberOfRequests + 1).Times(async () =>
            await client.GetAsync("/")
        );

        await client.GetAsync("/");

        await Task.Delay(options.Window + TimeSpan.FromMilliseconds(400));

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

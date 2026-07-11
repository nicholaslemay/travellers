using System.Net;
using FluentAssertions;

namespace TravellersTest;

public class RootEndpointTests(TravellersWebApplicationFactory factory) : IClassFixture<TravellersWebApplicationFactory>
{
    [Fact]
    public async Task ShouldReturnHelloWorldWhenGettingRoot()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Be("Hello World!");
    }
}

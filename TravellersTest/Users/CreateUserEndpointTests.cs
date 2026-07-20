using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Travellers.Users;
using TravellersTest.Support;

namespace TravellersTest.Users;

[Collection("Database")]
public class CreateUserEndpointTests(DatabaseMigrationFixture fixture) : DatabaseTest(fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ShouldCreateUserAndReturnCreatedResponse()
    {
        var client = CreateHttpClient();

        var response = await client.PostAsJsonAsync("/users", new CreateUserRequest("traveller@example.com"));

        var user = await response.Content.ReadFromJsonAsync<User>(JsonOptions);

        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        user.Should().NotBeNull();
        user!.Email.Should().Be("traveller@example.com");
        user.Id.Value.Should().BePositive();
    }

    [Fact]
    public async Task ShouldReturnValidationProblemWhenEmailIsEmpty()
    {
        var client = CreateHttpClient();

        var response = await client.PostAsJsonAsync("/users", new CreateUserRequest(""));

        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Email");
    }

    [Fact]
    public async Task ShouldReturnValidationProblemWhenEmailIsMalformed()
    {
        var client = CreateHttpClient();

        var response = await client.PostAsJsonAsync("/users", new CreateUserRequest("not-an-email"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnValidationProblemWhenEmailExceedsMaximumLength()
    {
        var client = CreateHttpClient();
        var tooLongEmail = $"{new string('a', 310)}@example.com";

        var response = await client.PostAsJsonAsync("/users", new CreateUserRequest(tooLongEmail));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

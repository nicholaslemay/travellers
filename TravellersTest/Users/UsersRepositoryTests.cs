using FluentAssertions;
using FluentAssertions.Execution;
using Travellers.Users;

namespace TravellersTest.Users;

[Collection("Database")]
public class UsersRepositoryTests(DatabaseMigrationFixture fixture)
    : TransactionalDatabaseTest(fixture)
{
    private static readonly DateTimeOffset Now =
        new(2026, 7, 12, 8, 30, 0, TimeSpan.Zero);

    private UsersRepository CreateRepository() =>
        new(DbContext, new FixedTimeProvider(Now));

    [Fact]
    public void ShouldCreateUserAndReturnFullyBuiltUser()
    {
        var repository = CreateRepository();

        var user = repository.CreateUser("traveller@example.com");

        using (new AssertionScope())
        {
            user.Id.Value.Should().BePositive();
            user.Email.Should().Be("traveller@example.com");
            user.CreatedAt.Should().Be(Now);
            user.UpdatedAt.Should().Be(Now);
        }
    }

    [Fact]
    public void ShouldGetUserById()
    {
        var repository = CreateRepository();
        var created = repository.CreateUser("traveller@example.com");

        var found = repository.GetById(created.Id);

        found.Should().Be(created);
    }

    [Fact]
    public void ShouldReturnNullWhenUserNotFound()
    {
        var repository = CreateRepository();

        var found = repository.GetById(new UserId(999999));

        found.Should().BeNull();
    }
}

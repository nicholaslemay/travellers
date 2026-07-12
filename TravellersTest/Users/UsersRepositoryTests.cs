using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Polly;
using Travellers.Support.Db;
using Travellers.Users;

namespace TravellersTest.Users;

[Collection("Database")]
public class UsersRepositoryTests(DatabaseMigrationFixture fixture)
    : TransactionalDatabaseTest(fixture)
{
    private static readonly DateTimeOffset Now =
        new(2026, 7, 12, 8, 30, 0, TimeSpan.Zero);

    private UsersRepository CreateRepository() =>
        new(DbContext, new FixedTimeProvider(Now), new DatabaseExecutor(ResiliencePipeline.Empty));

    [Fact]
    public void ShouldCreateUserAndReturnFullyBuiltUser()
    {
        var repository = CreateRepository();

        var user = repository.CreateUser("traveller@example.com");

        using var _ = new AssertionScope();
        user.Id.Value.Should().BePositive();
        user.Email.Should().Be("traveller@example.com");
        user.CreatedAt.Should().Be(Now);
        user.UpdatedAt.Should().Be(Now);
    }
    
    [Fact]
    public void ShouldEnforceUniqueEmail()
    {
        var repository = CreateRepository();
        repository.CreateUser("traveller@example.com");
        Action createAnotherUser = () => repository.CreateUser("traveller@example.com");

        createAnotherUser.Should().Throw<DbUpdateException>();
    }
    
    [Fact]
    public void ShouldGenerateDifferentUserIds()
    {
        var repository = CreateRepository();
        var user1 = repository.CreateUser("user1@example.com");
        var user2 = repository.CreateUser("user2@example.com");

        user1.Id.Should().NotBe(user2.Id);
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

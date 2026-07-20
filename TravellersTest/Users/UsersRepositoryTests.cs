using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Polly;
using Travellers.Support.Db;
using Travellers.Users;
using TravellersTest.Support;

namespace TravellersTest.Users;

[Collection("Database")]
public class UsersRepositoryTests(DatabaseMigrationFixture fixture)
    : DatabaseTest(fixture)
{
    private readonly DateTimeOffset _now = new(2026, 7, 12, 8, 30, 0, TimeSpan.Zero);

    private UsersRepository CreateRepository() => (UsersRepository)GetService<IUserRepository>();

    [Fact]
    public async Task ShouldCreateUserAndReturnFullyBuiltUser()
    {
        var repository = CreateRepository();
        FakeTime.AdjustTime(_now);
        
        var user = await repository.CreateUserAsync("traveller@example.com");

        using var _ = new AssertionScope();
        user.Id.Value.Should().BePositive();
        user.Email.Should().Be("traveller@example.com");
        user.CreatedAt.Should().Be(_now);
        user.UpdatedAt.Should().Be(_now);
    }

    [Fact]
    public async Task ShouldEnforceUniqueEmail()
    {
        var repository = CreateRepository();
        await repository.CreateUserAsync("traveller@example.com");
        Func<Task> createAnotherUser = () => repository.CreateUserAsync("traveller@example.com");

        await createAnotherUser.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task ShouldGenerateDifferentUserIds()
    {
        var repository = CreateRepository();
        var user1 = await repository.CreateUserAsync("user1@example.com");
        var user2 = await repository.CreateUserAsync("user2@example.com");

        user1.Id.Should().NotBe(user2.Id);
    }

    [Fact]
    public async Task ShouldGetUserById()
    {
        var repository = CreateRepository();
        var created = await repository.CreateUserAsync("traveller@example.com");

        var found = await repository.GetByIdAsync(created.Id);

        found.Should().Be(created);
    }

    [Fact]
    public async Task ShouldReturnNullWhenUserNotFound()
    {
        var repository = CreateRepository();

        var found = await repository.GetByIdAsync(new UserId(999999));

        found.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReportEmailExistsAfterUserIsCreated()
    {
        var repository = CreateRepository();
        await repository.CreateUserAsync("traveller@example.com");

        var exists = await repository.ExistsByEmailAsync("traveller@example.com");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReportEmailDoesNotExistWhenNoUserHasIt()
    {
        var repository = CreateRepository();

        var exists = await repository.ExistsByEmailAsync("nobody@example.com");

        exists.Should().BeFalse();
    }
}

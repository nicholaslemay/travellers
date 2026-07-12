namespace Travellers.Users;

public record UserId(int Value);

public record User(UserId Id, string Email, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

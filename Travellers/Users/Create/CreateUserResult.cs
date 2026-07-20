using Travellers.Users;

namespace Travellers.Users.Create;

public record CreateUserResult
{
    public bool Succeeded { get; private init; }
    public User? User { get; private init; }
    public bool EmailWasAlreadyTaken { get; private init; }

    public static CreateUserResult Created(User user) => new() { Succeeded = true, User = user };

    public static CreateUserResult EmailAlreadyExists() => new() { Succeeded = false, EmailWasAlreadyTaken = true };
}

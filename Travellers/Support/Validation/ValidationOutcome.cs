namespace Travellers.Support.Validation;

public record ValidationOutcome(bool IsValid, IReadOnlyDictionary<string, string[]> Errors)
{
    public static ValidationOutcome Success() => new(true, new Dictionary<string, string[]>());

    public static ValidationOutcome Failure(IReadOnlyDictionary<string, string[]> errors) => new(false, errors);
}

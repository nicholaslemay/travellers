namespace Travellers.Support.Db;

public record DatabaseResilienceOptions
{
    public TimeSpan Timeout { get; init; }
}

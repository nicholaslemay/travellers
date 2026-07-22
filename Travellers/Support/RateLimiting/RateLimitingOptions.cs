namespace Travellers.Support.RateLimiting;

public record RateLimitingOptions
{
    public int PermitLimit { get; set; }
    public TimeSpan Window { get; set; }
}

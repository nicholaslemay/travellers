namespace Travellers.Support;

public abstract record Lookup<T>
{
    public sealed record Found(T Value) : Lookup<T>;
    public sealed record NotFound : Lookup<T>;
}

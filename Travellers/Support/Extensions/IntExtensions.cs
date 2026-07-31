namespace Travellers.Support.Extensions;

public static class IntExtensions
{
    public static void Times(this int count, Action action)
    {
        for (int i = 0; i < count; i++)
        {
            action();
        }
    }
    
    
    public static async Task Times(this int count, Func<Task> task)
    {
        for (int i = 0; i < count; i++)
        {
            await task();
        }
    }
}
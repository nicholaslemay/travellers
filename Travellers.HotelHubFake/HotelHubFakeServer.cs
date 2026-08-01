namespace Travellers.HotelHubFake;

public class HotelHubFakeServer : IDisposable
{
    private readonly WebApplication _app;

    public string BaseUrl { get; }

    public HotelHubFakeServer()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        _app = builder.Build();
        HotelHubFakeApp.MapEndpoints(_app);
        _app.Start();
        BaseUrl = _app.Urls.First();
    }

    public void Dispose()
    {
        _app.StopAsync().GetAwaiter().GetResult();
        _app.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}

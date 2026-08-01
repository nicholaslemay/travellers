namespace Travellers.HotelHubFake;

public class HotelHubFakeServer : IDisposable
{
    private readonly WebApplication _app;
    private readonly FakeReservationStore _store = new();

    public string BaseUrl { get; }

    public HotelHubFakeServer()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        _app = builder.Build();
        HotelHubFakeApp.MapEndpoints(_app, _store);
        _app.Start();
        BaseUrl = _app.Urls.First();
    }

    public void AddReservation(string reservationId, string hotelName, DateOnly checkIn, DateOnly checkOut) =>
        _store.Add(reservationId, new FakeReservation(hotelName, checkIn, checkOut));

    public void Dispose()
    {
        _app.StopAsync().GetAwaiter().GetResult();
        _app.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}

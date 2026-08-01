using Travellers.HotelHubFake;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var store = new FakeReservationStore();
store.Add("HB-DEMO-001", new FakeReservation("Le Méridien Paris", new DateOnly(2026, 8, 15), new DateOnly(2026, 8, 18)));
store.Add("HB-DEMO-002", new FakeReservation("Hotel Le Bristol Paris", new DateOnly(2026, 8, 19), new DateOnly(2026, 8, 22)));

HotelHubFakeApp.MapEndpoints(app, store);

app.Run();

public partial class Program { }

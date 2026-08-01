using Travellers.HotelHubFake;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

HotelHubFakeApp.MapEndpoints(app);

app.Run();

public partial class Program { }

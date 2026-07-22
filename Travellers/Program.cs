using Travellers.Support.Db;
using Travellers.Support.RateLimiting;
using Travellers.Users;
using Travellers.Users.Create;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTravellersDatabase(builder.Configuration)
    .AddTravellersRateLimiting(builder.Configuration)
    .AddUsersModule();

var app = builder.Build();

app.UseRateLimiter();

DatabaseMigrator.Migrate(app.Configuration.GetConnectionString("TravellersDb")!);

app.MapGet("/", () => "Hello World!");
app.MapCreateUserEndpoint();

app.Run();

public partial class Program { }
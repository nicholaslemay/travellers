using Travellers.Support.Db;
using Travellers.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTravellersDatabase(builder.Configuration)
    .AddUsersModule();

var app = builder.Build();

DatabaseMigrator.Migrate(app.Configuration.GetConnectionString("TravellersDb")!);

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program { }
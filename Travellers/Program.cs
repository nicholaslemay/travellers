using Travellers.Support.Db;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

DatabaseMigrator.Migrate(app.Configuration.GetConnectionString("TravellersDb")!);

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program { }
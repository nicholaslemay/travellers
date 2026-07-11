using Microsoft.Extensions.Configuration;
using Travellers.Support.Db;

namespace TravellersTest;

public class DatabaseMigrationFixture
{
    public string ConnectionString { get; }

    public DatabaseMigrationFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        ConnectionString = configuration.GetConnectionString("TravellersDb")!;

        DatabaseMigrator.Migrate(ConnectionString);
    }
}

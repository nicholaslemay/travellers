using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;

namespace Travellers.Support.Db;

public static class DatabaseMigrator
{
    public static void Migrate(string connectionString)
    {
        EnsureDatabaseExists(connectionString);

        using var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(DatabaseMigrator).Assembly).For.Migrations())
            .BuildServiceProvider(false);

        using var scope = serviceProvider.CreateScope();
        var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrationRunner.MigrateUp();
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        var databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        var masterConnectionString = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master"
        }.ConnectionString;

        using var connection = new SqlConnection(masterConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            IF DB_ID(@databaseName) IS NULL
            BEGIN
                DECLARE @createDatabase nvarchar(max) = N'CREATE DATABASE ' + QUOTENAME(@databaseName);
                EXEC (@createDatabase);
            END
            """;
        command.Parameters.Add(new SqlParameter("@databaseName", databaseName));
        command.ExecuteNonQuery();
    }
}

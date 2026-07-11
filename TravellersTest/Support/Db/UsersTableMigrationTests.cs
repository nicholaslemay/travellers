using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Data.SqlClient;

namespace TravellersTest.Support.Db;

public class UsersTableMigrationTests(DatabaseMigrationFixture fixture)
    : IClassFixture<DatabaseMigrationFixture>
{
    [Fact]
    public void ShouldCreateUsersTableWithIdentityUserId()
    {
        using var connection = new SqlConnection(fixture.ConnectionString);
        connection.Open();

        var tableCount = ExecuteScalar(connection,
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table",
            ("@table", "users"));

        var columnCount = ExecuteScalar(connection,
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = @column",
            ("@table", "users"), ("@column", "user_id"));

        var isIdentity = ExecuteScalar(connection,
            "SELECT COLUMNPROPERTY(OBJECT_ID(@table), @column, 'IsIdentity')",
            ("@table", "users"), ("@column", "user_id"));

        using (new AssertionScope())
        {
            tableCount.Should().Be(1);
            columnCount.Should().Be(1);
            isIdentity.Should().Be(1);
        }
    }

    private static int ExecuteScalar(
        SqlConnection connection,
        string sql,
        params (string Name, string Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var (name, value) in parameters)
            command.Parameters.Add(new SqlParameter(name, value));
        return Convert.ToInt32(command.ExecuteScalar());
    }
}

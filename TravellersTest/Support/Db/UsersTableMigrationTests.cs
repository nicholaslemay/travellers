using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Data.SqlClient;

namespace TravellersTest.Support.Db;

[Collection("Database")]
public class UsersTableMigrationTests(DatabaseMigrationFixture fixture)
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

    [Fact]
    public void ShouldAddEmailAndTimestampColumnsToUsers()
    {
        using var connection = new SqlConnection(fixture.ConnectionString);
        connection.Open();

        var emailColumnCount = ExecuteScalar(connection,
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = @column",
            ("@table", "users"), ("@column", "email"));

        var createdAtIsNotNullable = ExecuteScalar(connection,
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = @column AND IS_NULLABLE = @nullable",
            ("@table", "users"), ("@column", "created_at"), ("@nullable", "NO"));

        var updatedAtIsNotNullable = ExecuteScalar(connection,
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = @column AND IS_NULLABLE = @nullable",
            ("@table", "users"), ("@column", "updated_at"), ("@nullable", "NO"));

        using (new AssertionScope())
        {
            emailColumnCount.Should().Be(1);
            createdAtIsNotNullable.Should().Be(1);
            updatedAtIsNotNullable.Should().Be(1);
        }
    }

    [Fact]
    public void ShouldEnforceUniqueEmail()
    {
        using var connection = new SqlConnection(fixture.ConnectionString);
        connection.Open();

        var uniqueConstraintCount = ExecuteScalar(connection,
            """
            SELECT COUNT(*)
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
            JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
                ON tc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
            WHERE tc.TABLE_NAME = @table
                AND tc.CONSTRAINT_TYPE = 'UNIQUE'
                AND ccu.COLUMN_NAME = @column
            """,
            ("@table", "users"), ("@column", "email"));

        uniqueConstraintCount.Should().Be(1);
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

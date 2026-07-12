using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Travellers.Support.Db;

namespace TravellersTest;

public abstract class TransactionalDatabaseTest : IDisposable
{
    private readonly SqlConnection _connection;
    private readonly IDbContextTransaction _transaction;

    protected TravellersDbContext DbContext { get; }

    protected TransactionalDatabaseTest(DatabaseMigrationFixture fixture)
    {
        _connection = new SqlConnection(fixture.ConnectionString);
        _connection.Open();

        var options = new DbContextOptionsBuilder<TravellersDbContext>()
            .UseSqlServer(_connection)
            .Options;

        DbContext = new TravellersDbContext(options);
        _transaction = DbContext.Database.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction.Rollback();
        _transaction.Dispose();
        DbContext.Dispose();
        _connection.Dispose();
    }
}

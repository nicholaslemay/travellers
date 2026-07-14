using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using Travellers.Support.Db;

namespace TravellersTest.Support;

public abstract class DatabaseTest : IAsyncLifetime
{
    private readonly List<Action<IServiceCollection>> _serviceOverrides = [];
    private WebApplicationFactory<Program>? _factory;
    private IServiceScope? _scope;
    private IDbContextTransaction? _transaction;

    protected FakeTimeProvider FakeTime { get; } = new();

    protected DatabaseTest(DatabaseMigrationFixture fixture)
    {
        // Unused, but required so xUnit resolves the "Database" collection fixture
        // (and therefore runs migrations) before this test executes.
        _ = fixture;
    }

    protected void OverrideServices(Action<IServiceCollection> configure) =>
        _serviceOverrides.Add(configure);

    protected TravellersDbContext DbContext => GetService<TravellersDbContext>();

    protected T GetService<T>() where T : notnull =>
        EnsureScope().ServiceProvider.GetRequiredService<T>();

    private IServiceScope EnsureScope()
    {
        if (_scope is not null) return _scope;

        _factory = new TravellersWebApplicationFactory().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(FakeTime);

                foreach (var configure in _serviceOverrides) configure(services);
            }));

        _scope = _factory.Services.CreateScope();
        _transaction = _scope.ServiceProvider
            .GetRequiredService<TravellersDbContext>()
            .Database.BeginTransaction();

        return _scope;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (_transaction is not null)
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            catch (Exception exception) when (exception is InvalidOperationException or SqlException)
            {
                // The connection may already be broken (e.g. a timeout test that
                // aborted mid-command) - cleanup should never mask the real
                // assertion failure.
            }
            finally
            {
                await _transaction.DisposeAsync();
            }
        }

        _scope?.Dispose();

        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }
}

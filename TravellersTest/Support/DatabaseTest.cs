using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

    private readonly FaultInjectingCommandInterceptor _interceptor = new();
    private readonly DatabaseMigrationFixture _fixture;

    protected FakeTimeProvider FakeTime { get; } = new();

    protected DatabaseTest(DatabaseMigrationFixture fixture)
    {
        _fixture = fixture;
    }

    protected void OverrideServices(Action<IServiceCollection> configure) =>
        _serviceOverrides.Add(configure);

    protected void SetupDatabaseToHang() => _interceptor.Hang();

    protected TravellersDbContext DbContext => GetService<TravellersDbContext>();

    protected T GetService<T>() where T : notnull =>
        EnsureScope().ServiceProvider.GetRequiredService<T>();

    protected HttpClient CreateHttpClient()
    {
        EnsureScope();

        return _factory!.CreateClient();
    }

    private IServiceScope EnsureScope()
    {
        if (_scope is not null) return _scope;

        var dbContext = new TravellersDbContext(new DbContextOptionsBuilder<TravellersDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .AddInterceptors(_interceptor)
            .Options);

        _transaction = dbContext.Database.BeginTransaction();

        _factory = new TravellersWebApplicationFactory().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(FakeTime);

                services.RemoveAll<DbContextOptions<TravellersDbContext>>();
                services.RemoveAll<TravellersDbContext>();
                services.AddSingleton(dbContext);

                foreach (var configure in _serviceOverrides) configure(services);
            }));

        _scope = _factory.Services.CreateScope();

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

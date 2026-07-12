using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;

namespace Travellers.Support.Db;

public static class DatabaseServices
{
    public static IServiceCollection AddTravellersDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.Configure<DatabaseResilienceOptions>(
            configuration.GetSection("DatabaseResilience"));

        services.AddResiliencePipeline("database", (builder, context) =>
        {
            var options = context.ServiceProvider
                .GetRequiredService<IOptions<DatabaseResilienceOptions>>().Value;
            builder.TimeProvider = context.ServiceProvider.GetRequiredService<TimeProvider>();
            DatabaseResiliencePipeline.Configure(builder, options);
        });

        services.AddScoped(serviceProvider => new DatabaseExecutor(
            serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>()
                .GetPipeline("database")));

        services.AddDbContext<TravellersDbContext>((serviceProvider, options) =>
            options
                .UseSqlServer(configuration.GetConnectionString("TravellersDb"))
                .AddInterceptors(serviceProvider.GetServices<IInterceptor>()));

        return services;
    }
}

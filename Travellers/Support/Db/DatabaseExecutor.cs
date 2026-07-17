using Microsoft.Data.SqlClient;
using Polly;
using Polly.Timeout;

namespace Travellers.Support.Db;

public class DatabaseExecutor(TravellersDbContext dbContext, ResiliencePipeline pipeline)
{
    public async Task<T> ExecuteAsync<T>(
        Func<TravellersDbContext, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await pipeline.ExecuteAsync(
                async token => await ExecuteHonouringCancellation(operation, token).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);
        }
        catch (TimeoutRejectedException exception)
        {
            throw new DatabaseTimeoutException(exception);
        }
    }

    private async Task<T> ExecuteHonouringCancellation<T>(
        Func<TravellersDbContext, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        try
        {
            return await operation(dbContext, cancellationToken).ConfigureAwait(false);
        }
        catch (SqlException) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }
}

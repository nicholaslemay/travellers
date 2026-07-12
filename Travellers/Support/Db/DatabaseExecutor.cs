using Microsoft.Data.SqlClient;
using Polly;
using Polly.Timeout;

namespace Travellers.Support.Db;

public class DatabaseExecutor(ResiliencePipeline pipeline)
{
    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
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

    private static async Task<T> ExecuteHonouringCancellation<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        try
        {
            return await operation(cancellationToken).ConfigureAwait(false);
        }
        catch (SqlException) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }
}

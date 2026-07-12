using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TravellersTest.Users;

public sealed class HangingCommandInterceptor : DbCommandInterceptor
{
    private readonly TaskCompletionSource _commandStarted =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task CommandStarted => _commandStarted.Task;

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        command.CommandText = "WAITFOR DELAY '00:10:00';" + command.CommandText;
        _commandStarted.TrySetResult();

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }
}

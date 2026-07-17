using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TravellersTest.Support;

public sealed class FaultInjectingCommandInterceptor : DbCommandInterceptor
{
    private Action<DbCommand> _behavior = _ => { };

    public void Hang() => _behavior = command =>
        command.CommandText = "WAITFOR DELAY '00:10:00';" + command.CommandText;

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        _behavior(command);

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _behavior(command);

        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }
}

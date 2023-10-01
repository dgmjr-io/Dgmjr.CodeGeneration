namespace Dgmjr.CodeGeneration.Logging;

using System.ComponentModel;
using System.Data.Common;
using System.Transactions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static LogEvents;
using static Microsoft.Extensions.Logging.LogLevel;

internal static partial class LoggingExtensions
{
    private static readonly Action<ILogger, string, Exception?> _LogNewScope =
        LoggerMessage.Define<string>(Trace, TransactionScopeStarted.Id, "{0}: ");
    private static readonly Action<
        ILogger,
        IIncrementalGenerator,
        DateTimeOffset,
        Exception?
    > _beginLog = LoggerMessage.Define<IIncrementalGenerator, DateTimeOffset>(
        Trace,
        BeginLog.Id,
        "Begin Log for {0} {1:u}"
    );
    private static readonly Action<ILogger, string, DateTimeOffset, Exception?> _beginLog2 =
        LoggerMessage.Define<string, DateTimeOffset>(Trace, BeginLog.Id, "Begin Log for {0} {1:u}");

    public static void LogNewScope(this ILogger logger, string newScope) =>
        _LogNewScope(logger, newScope, null);

    public static void LogBeginLog(
        this ILogger logger,
        IIncrementalGenerator generator,
        DateTimeOffset dto
    ) => _beginLog(logger, generator, dto.ToLocalTime(), null);

    public static void LogBeginLog(this ILogger logger, string generator, DateTimeOffset dto) =>
        _beginLog2(logger, generator, dto.ToLocalTime(), null);
}

internal static partial class LogEvents
{
    public static class TransactionScopeStarted
    {
        public const int Id = 1;
        public const string Name = "TransactionScopeStarted";
        public static readonly EventId Instance = new(Id, Name);
    }

    public static class BeginLog
    {
        public const int Id = 0;
        public const string Name = "BeginLog";
        public static readonly EventId Instance = new(Id, Name);
    }
}

namespace Dgmjr.CodeGeneration.CompileTimeComputation.Tests;
using Microsoft.CodeAnalysis;

using static Microsoft.Extensions.Logging.LogLevel;

public static partial class LoggerExtensions
{
    [LoggerMessage(-1, Error, "{Location} ({Id}): {Descriptor}")]
    public static partial void LogDiagnosticError(this ILogger logger, Location location, string id, DiagnosticDescriptor descriptor);
    [LoggerMessage(1, Information, "Source to be compiled:\r\n\r\n {Source}")]
    public static partial void LogSourceToBeCompiled(this ILogger logger, string source);
}

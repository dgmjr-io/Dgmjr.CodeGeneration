using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dgmjr.CodeGeneration.Logging;

public class SourceGeneratorLoggingProvider : ILoggerProvider
{
    private bool disposedValue;
    private readonly IncrementalGeneratorInitializationContext _context;

    public SourceGeneratorLoggingProvider(IncrementalGeneratorInitializationContext context)
    {
        _context = context;
    }

    public ILogger CreateLogger(string? categoryName)
    {
        return new SourceGeneratorLogger(_context, categoryName);
    }

    public ILogger<T> CreateLogger<T>()
        where T : notnull => new SourceGeneratorLogger<T>(_context);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // noop
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

using Microsoft.Extensions.Logging;

namespace Dgmjr.CodeGeneration.Logging;

public class SourceGeneratorLoggerScope<TScope> : IDisposable
    where TScope : notnull
{
    private readonly Action _endScope;

    public SourceGeneratorLoggerScope(ILogger logger, Action endScope)
    {
        _endScope = endScope;
        logger.LogNewScope(typeof(TScope).Name);
    }

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _endScope();
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

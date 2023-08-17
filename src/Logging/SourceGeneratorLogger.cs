namespace Dgmjr.CodeGeneration.Logging;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;

public class SourceGeneratorLogger<T> : SourceGeneratorLogger, ILogger<T> where T : notnull
{
    public SourceGeneratorLogger(IncrementalGeneratorInitializationContext context) : base(context, typeof(T).FullName) { }
}

public class SourceGeneratorLogger : ILogger, IDisposable
{
    private int _indentation = 0;
    private string _strIndentation = "";
    private bool disposedValue;

    protected string Indentation => _strIndentation;
    private readonly string? _category;
    private readonly IncrementalGeneratorInitializationContext _context;
    private readonly IList<string> _logs = new List<string>();

    public SourceGeneratorLogger(IncrementalGeneratorInitializationContext context, string? category = null)
    {
        _context = context;
        _category = category;
        this.LogBeginLog(_category, DateTimeOffset.UtcNow);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        _indentation++;
        ReIndent();
        return new SourceGeneratorLoggerScope<TState>(this, EndScope);
    }

    private void EndScope()
    {
        _indentation--;
        ReIndent();
    }

    private void ReIndent()
    {
        var indentationBuilder = new StringBuilder();
        for (var i = 0; i < _indentation; i++)
        {
            indentationBuilder.Append("  ");
        }
        _strIndentation = indentationBuilder.ToString();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logs.Add($"/* {Indentation}{formatter(state, exception)} */");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _context.RegisterPostInitializationOutput(ctx => ctx.AddSource("log.g.cs", GetLogs()));
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

    private string GetLogs() => Join(env.NewLine, _logs);
}

namespace Microsoft.CodeAnalysis;

using System.Runtime.Serialization;

public class CompilationException : Exception
{
    public CompilationException() { }

    public CompilationException(string message)
        : base(message) { }

    public CompilationException(string message, Exception innerException)
        : base(message, innerException) { }

    protected CompilationException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}

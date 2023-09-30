namespace Dgmjr.CodeGeneration.CompileTimeComputation.Tests;

public static class Constants
{
    public const string Source =
    """
    namespace Foo;

    public partial class Bar
    {
        public static readonly MD5 MD5 = global::System.Security.Cryptography.MD5.Create();

        public const string UriString = "https://dgmjr.io/codegeneration/compiletimecomputation/samples";

        [method: CompileTimeComputation("GuidString")]
        public static string MakeGuidString() => MD5.ComputeHash(UriString.ToUTF8Bytes()).ToHexString();
    }
    """;

    public const string Bar = nameof(Bar);
    public const string Foo = nameof(Foo);
    public const string GuidString = nameof(GuidString);
}

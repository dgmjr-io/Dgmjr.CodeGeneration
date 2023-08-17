namespace Dgmjr.CodeGeneration.CompileTimeComputation.Samples;
using System.Security.Cryptography;
using System.Text;

public partial class Foo
{
    public static readonly MD5 MD5 = MD5.Create();

    public static readonly CompileTimeComputation<string> _guidString = new CompileTimeComputation<string>("GuidString", () => MakeGuidString());

    public const string UriString = "https://dgmjr.io/codegeneration/compiletimecomputation/samples";

    public static string MakeGuidString() => MD5.ComputeHash(UriString.ToUTF8Bytes()).ToHexString();
}

public sealed class CompileTimeComputation<T>
{
    public CompileTimeComputation(string name, Func<T> compute, bool isPublic = true)
    {
        Name = name;
        Compute = compute;
        IsPublic = isPublic;
    }


    public string Name { get; }
    public Func<T> Compute { get; }
    public bool IsPublic { get; }
}

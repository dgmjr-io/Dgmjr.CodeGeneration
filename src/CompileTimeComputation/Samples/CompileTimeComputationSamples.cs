namespace Dgmjr.CodeGeneration.CompileTimeComputation.Samples;

using System;
using System.Security.Cryptography;
using System.Text;

// using CompileTimeComputation<T> = Dgmjr.CodeGeneration.CompileTimeComputation.Samples.Bar.CompileTimeComputation<T>;

public static partial class Foo
{
    public static readonly MD5 MD5 = global::System.Security.Cryptography.MD5.Create();
    public const string UriString = "https://dgmjr.io/codegeneration/compiletimecomputation/samples";

    [method: CompileTimeComputation("GuidString")]
    public static string MakeGuidString() => MD5.ComputeHash(UriString.ToUTF8Bytes()).ToHexString();
}

public static partial class Foo
{
}

// public sealed class CompileTimeComputationAttribute<T> : Attribute
// {
//     public CompileTimeComputationAttribute(string name)
//     {
//         Name = name;
//     }

//     public string Name { get; }
// }

// namespace Bar
// {
// public sealed class CompileTimeComputation<T>
// {
//     public CompileTimeComputation(string name, Func<T> compute, bool isPublic = true)
//     {
//         Name = name;
//         Compute = compute;
//         IsPublic = isPublic;
//     }


//     public string Name { get; }
//     public Func<T> Compute { get; }
//     public bool IsPublic { get; }
// }
// }

namespace Dgmjr.CodeGeneration.CompileTimeComputation.Samples;

using System;
using System.Security.Cryptography;
using System.Text;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
// using Microsoft.CodeAnalysis.MSBuild;

public static partial class Foo
{
    public static readonly MD5 MD5 = global::System.Security.Cryptography.MD5.Create();
    public const string UriString = "https://dgmjr.io/codegeneration/compiletimecomputation/samples";

    [method: CompileTimeComputation("GuidString")]
    public static string MakeGuidString() => MD5.ComputeHash(UriString.ToUTF8Bytes()).ToHexString();
}

// public static partial class Foo
// {
//     static Foo()
//     {
//         var workspace = MSBuildWorkspace.Create();
//         var project = workspace.OpenProjectAsync("/Users/david/GitHub/dgmjr-io/src/CodeGeneration/src/CompileTimeComputation/Samples/Dgmjr.CodeGeneration.CompileTimeComputation.Samples.csproj").GetAwaiter().GetResult();
//         var compilation = project.GetCompilationAsync().Result;
//         var methodSymbol = compilation.SyntaxTrees.SelectMany(st => st.GetCompilationUnitRoot().DescendantNodesAndSelf(n => n is MethodDeclarationSyntax, true)).Select(m => compilation.GetSemanticModel(m.SyntaxTree).GetDeclaredSymbol(m)).FirstOrDefault(ms => ms is IMethodSymbol methSym && methSym.Name == "MakeGuidString") as IMethodSymbol;
//         Console.WriteLine(methodSymbol.InvokeStaticMethod(compilation));
//     }
// }

public sealed class CompileTimeComputationAttribute : Attribute
{
    public CompileTimeComputationAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

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

using System.Linq;
namespace Dgmjr.CodeGeneration.CompileTimeComputation;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Security.Cryptography;

using Dgmjr.CodeGeneration.MethodSymbolExtensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

using static Constants;

using FieldDeclaration = (IPropertySymbol? Property, IMethodSymbol? Method, string Name, IFieldSymbol FieldSymbol, INamedTypeSymbol FieldType);
using FieldDeclarationSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax;
using VariableDeclaratorSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax;

[Generator]
public class CompileTimeComputationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterPostInitializationOutput = context.RegisterPostInitializationOutput;
        RegisterPostInitializationOutput(ctx => ctx.AddSource($"{CompileTimeComputation}.g.cs",
            HeaderTemplate.Render(new { Filename = $"{CompileTimeComputation}.g.cs" }) +
            CompileTimeComputationClassDeclaration));
        // Register the generator for syntax notifications
        var syntaxProvider = context.SyntaxProvider.ForAttributeWithMetadataName(CompileTimeComputation, Select, Project);
        var syntaxProviderWithCompilationProvider = syntaxProvider.Collect().Combine(context.CompilationProvider);
        context.RegisterSourceOutput(syntaxProviderWithCompilationProvider, Execute);
    }

    private static Action<Action<IncrementalGeneratorPostInitializationContext>> RegisterPostInitializationOutput { get; set; }

    private static bool Select(SyntaxNode node, CancellationToken cancellationToken)
        => node is PropertyDeclarationSyntax pds ? pds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.PublicKeyword)).Any() && pds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.StaticKeyword)).Any() && pds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.ReadOnlyKeyword)).Any() :
            node is MethodDeclarationSyntax mds && mds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.PublicKeyword)).Any() && mds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.StaticKeyword)).Any();

    private static FieldDeclaration Project(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        => (Property: context.TargetNode is PropertyDeclarationSyntax ? context.TargetSymbol as IPropertySymbol : null,
            Method: context.TargetNode is PropertyDeclarationSyntax ? (context.TargetSymbol as IPropertySymbol).GetMethod : context.TargetNode is MethodDeclarationSyntax ? context.TargetSymbol as IMethodSymbol : null,
            Name: (context.Attributes.FirstOrDefault(a => a.AttributeClass.Name == CompileTimeComputation).ConstructorArguments.FirstOrDefault().Value as string)!,
            FieldSymbol: (context.TargetSymbol as IFieldSymbol)!,
            FieldType: ((context.TargetNode is PropertyDeclarationSyntax ? (context.TargetSymbol as IPropertySymbol).Type : context.TargetNode is MethodDeclarationSyntax ? (context.TargetSymbol as IMethodSymbol).ReturnType : null) as INamedTypeSymbol)!);

    public static void Execute(SourceProductionContext context, (ImmutableArray<FieldDeclaration> FieldDeclarations, Compilation Compilation) contexts)
    {
        var compilation = contexts.Compilation;
        foreach (var (property, method, name, fieldSymbol, fieldType) in contexts.FieldDeclarations)
        {
            // Get the field type and name
            var fieldSymbolDisplay = $"{fieldType.ToDisplayString()}: {fieldType.GetType()}";

            if (ValidConstTypeNames.Contains(fieldType?.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                // create the const variable expresion
                var funcResult = method.InvokeMethod(compilation, null!);
                var filename = $"CompileTimeComputation.{method.ContainingType}.{name}.g.cs";

                var constDeclaration =
                HeaderTemplate.Render(new { Filename = filename }) +
                $$$"""
                    namespace {{{fieldSymbol.ContainingType.ContainingNamespace.ToDisplayString() }
}};

public
{ { { (fieldSymbol.ContainingType.IsStatic ? "static" : "")} } }
partial
{ { { (fieldSymbol.ContainingType.IsRecord ? "record" : "")} } }
{ { { (fieldSymbol.ContainingType.TypeKind == TypeKind.Class ? "class" : fieldSymbol.ContainingType.TypeKind == TypeKind.Struct ? "struct" : $"#error Wrong data structure type: {fieldSymbol.ContainingType.TypeKind}")} } }
{ { { fieldSymbol.ContainingType.Name} } }
{
                        public const { { { fieldSymbol.ToDisplayString()} } }
{ { { name} } } = { { { (fieldType.Name.Equals("string", InvariantCultureIgnoreCase) ? "\"" : "")} } }
{ { { funcResult} } }
{ { { (fieldSymbol.Name.Equals("string", InvariantCultureIgnoreCase) ? "\"" : "")} } };
                    }
                    """;

                // Add the class and const variable declarations to the compilation
context.AddSource(filename, constDeclaration);
            }
            else
{
    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CTCG002", "Error generatimg compile-time computed constant: must be const-able",
    Format(CTCG002ErrorMessage, fieldSymbolDisplay),
    "CTCG002: Field must be const-able", DiagnosticSeverity.Error, true),
    fieldSymbol.Locations.FirstOrDefault()));
}
        }
    }

    private static object CompileAndRunFunc(SourceProductionContext context, Compilation compilation, INamedTypeSymbol returnType, IFieldSymbol fieldSymbol)
{
    try
    {
        var programClassName = $"Program_{guid.NewGuid().ToString().Substring(0, 4)}";
        var code =
            $$$"""
            using System;
            public class {{ { programClassName} }}
            {
                public static
{ { { returnType.ToDisplayString()} } }
Run()
                {
    return { { { fieldSymbol.ContainingType.ToDisplayString()} } }.{ { { fieldSymbol.Name} } }.Compute();
}
            }
            """;

            var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
var syntaxTree = CSharpSyntaxTree.ParseText(code, parseOptions);

compilation = compilation.AddSyntaxTrees(syntaxTree)
    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
    .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

using var peStream = new MemoryStream();
using var pdbSream = new MemoryStream();
var assembly = compilation.Emit(peStream, pdbSream, options: new EmitOptions(false, DebugInformationFormat.Pdb, tolerateErrors: true, includePrivateMembers: true, pdbChecksumAlgorithm: HashAlgorithmName.SHA512), cancellationToken: default);
if (assembly.Success)
{
    peStream.Flush();

    var dynamicAssembly = Assembly.Load(peStream.GetBuffer());
    var programType = dynamicAssembly.GetType(programClassName);
    var methodInfo = programType.GetMethod("Run");

    return methodInfo.Invoke(null, null);
}
else
{
    foreach (var diagnostic in assembly.Diagnostics)
    {
        context.ReportDiagnostic(diagnostic);
    }
}
        }
        catch (Exception ex)
{
    RegisterPostInitializationOutput(ctx => ctx.AddSource("error.g.cs", $@"/*
{ex.GetType()}: {ex.Message}
{ex.StackTrace}
*/"));
}
return "NO RESULT";
    }
}

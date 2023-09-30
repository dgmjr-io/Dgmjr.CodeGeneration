namespace Dgmjr.CodeGeneration.CompileTimeComputation;
using System;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Constants;

using FieldDeclaration = (IPropertySymbol? Property, IMethodSymbol? Method, string Name, IFieldSymbol FieldSymbol, INamedTypeSymbol FieldType);

[Generator]
public class CompileTimeComputationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterPostInitializationOutput = context.RegisterPostInitializationOutput;
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{CompileTimeComputation}.g.cs",
            HeaderTemplate.Render(new { Filename = $"{CompileTimeComputation}.g.cs" }) +
            CompileTimeComputationClassDeclaration));
        // Register the generator for syntax notifications
        var syntaxProvider = context.SyntaxProvider.ForAttributeWithMetadataName(CompileTimeComputation, Select, Project).Collect();
        var syntaxProviderWithCompilationProvider = syntaxProvider.Combine(context.CompilationProvider);
        context.RegisterSourceOutput(syntaxProviderWithCompilationProvider, Execute);
    }

    private static Action<Action<IncrementalGeneratorPostInitializationContext>> RegisterPostInitializationOutput { get; set; }

    private static bool Select(SyntaxNode node, CancellationToken cancellationToken)
        => true; /*node is PropertyDeclarationSyntax pds ? pds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.PublicKeyword)).Any() && pds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.StaticKeyword)).Any() : node is MethodDeclarationSyntax mds && mds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.PublicKeyword)).Any() && mds.DescendantNodesAndTokensAndSelf(node => node.IsKind(SyntaxKind.StaticKeyword)).Any();*/

    private static FieldDeclaration Project(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        => (Property: context.TargetNode is PropertyDeclarationSyntax ? context.TargetSymbol as IPropertySymbol : null,
            Method: context.TargetNode is PropertyDeclarationSyntax ? (context.TargetSymbol as IPropertySymbol).GetMethod : context.TargetNode is MethodDeclarationSyntax ? context.TargetSymbol as IMethodSymbol : null,
            Name: (context.Attributes.FirstOrDefault(a => a.AttributeClass.Name == CompileTimeComputation).ConstructorArguments.FirstOrDefault().Value as string)!,
            FieldSymbol: (context.TargetSymbol as IFieldSymbol)!,
            FieldType: ((context.TargetNode is PropertyDeclarationSyntax ? (context.TargetSymbol as IPropertySymbol).Type : context.TargetNode is MethodDeclarationSyntax ? (context.TargetSymbol as IMethodSymbol).ReturnType : null) as INamedTypeSymbol)!);

    public static void Execute(SourceProductionContext context, (ImmutableArray<FieldDeclaration> FieldDeclarations, Compilation Compilation) contexts)
    {
        var (fieldDeclarations, compilation) = contexts;
        context.AddSource("ConstantFieldDeclarations.cs", $"/* \n{compilation.GetDiagnostics().Select(diag => $"{diag.Severity} {diag.Descriptor.Id}: {diag.Descriptor.Title}: {diag.Descriptor.Description} [{diag.Location.SourceTree.FilePath}({diag.Location.GetLineSpan().StartLinePosition.Line},{diag.Location.GetLineSpan().StartLinePosition.Character})]").Join("\n")} \n\n {fieldDeclarations.Length} \n*/");
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("DEBUG01", "", $"DEBUG(compilation.GetDiagnostics().Length): *{compilation.GetDiagnostics().Length}*", "DEBUG", DiagnosticSeverity.Warning, true), null));
        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("DEBUG02", "", $"DEBUG(fieldDeclarations.Length): *{fieldDeclarations.Length}*", "DEBUG", DiagnosticSeverity.Warning, true), null));
        foreach (var (property, method, name, fieldSymbol, fieldType) in fieldDeclarations)
        {
            // Get the field type and name
            var fieldSymbolDisplay = $"{fieldType.ToDisplayString()}: {fieldType.GetType()}";
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("DEBUG03", $"DEBUG/fieldSymbolDisplay: {fieldSymbolDisplay}", "", "DEBUG", DiagnosticSeverity.Warning, true), null));

            if (ValidConstTypeNames.Contains(fieldType?.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                // create the const variable expresion
                try
                {
                    var funcResult = method.InvokeStaticMethod(compilation);
                    var filename = $"CompileTimeComputation.{method.ContainingType}.{name}.g.cs";

                    var constDeclaration =
                    HeaderTemplate.Render(new { Filename = filename }) +
                    $$$"""
                    namespace {{{fieldSymbol.ContainingType.ContainingNamespace.ToDisplayString() }
}};

{ { { fieldSymbol.ContainingType.DeclaredAccessibility.ToString().ToLower().Replace("or", " ").Replace("and", " ")} } }
{ { { (fieldSymbol.ContainingType.IsStatic ? "static" : "")} } }
partial
{ { { (fieldSymbol.ContainingType.IsRecord ? "record" : "")} } }
{ { { (fieldSymbol.ContainingType.TypeKind == TypeKind.Class ? "class" : fieldSymbol.ContainingType.TypeKind == TypeKind.Struct ? "struct" : $"#error Wrong data structure type: {fieldSymbol.ContainingType.TypeKind}")} } }
{ { { fieldSymbol.ContainingType.Name} } }
{
                        public const { { { fieldSymbol.ToDisplayString()} } }
{ { { name} } } = { { { (fieldType.Name.Equals(nameof(String), InvariantCultureIgnoreCase) ? "\"" : "")} } }
{ { { funcResult} } }
{ { { (fieldSymbol.Name.Equals(nameof(String), InvariantCultureIgnoreCase) ? "\"" : "")} } };
                    }
                    """;

                    // Add the class and const variable declarations to the compilation
context.AddSource(filename, constDeclaration);
                }
                catch (TargetInvocationException tiex)
{
    throw tiex.InnerException ?? tiex;
}
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
}

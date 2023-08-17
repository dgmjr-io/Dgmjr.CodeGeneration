namespace Dgmjr.CodeGeneration.CompileTimeComputation;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using static Constants;
using FieldDeclaration = (Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax FieldDeclaration, IFieldSymbol FieldSymbol);
using FieldDeclarationSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax;

[Generator]
public class CompileTimeComputationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{CompileTimeComputation}.g.cs",
            HeaderTemplate.Render(new { Filename = $"{CompileTimeComputation}.g.cs" }) +
            CompileTimeComputationClassDeclaration));
        // Register the generator for syntax notifications
        var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => node is FieldDeclarationSyntax && node.GetText().ToString().Contains(CompileTimeComputation),
            (ctx, _) => (FieldDeclarationSyntax: (ctx.Node as FieldDeclarationSyntax)!, FieldSymbol: (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, _) as IFieldSymbol)!));
        var syntaxProviderWithCompilationProvider = syntaxProvider.Collect().Combine(context.CompilationProvider);
        context.RegisterSourceOutput(syntaxProviderWithCompilationProvider, Execute);
    }

    public static void Execute(SourceProductionContext context, (ImmutableArray<FieldDeclaration> FieldDeclarations, Compilation Compilation) contexts)
    {
        var compilation = contexts.Compilation;
        foreach (var fieldDeclaration in contexts.FieldDeclarations.Select(fd => fd.FieldDeclaration))
        {
            // Get the field symbol
            var fieldSymbol = compilation.GetSemanticModel(fieldDeclaration.SyntaxTree).GetDeclaredSymbol(fieldDeclaration) as IFieldSymbol;
            var fieldSymbolDisplay = fieldSymbol?.ToDisplayString() ?? fieldDeclaration?.GetText()?.ToString();
            fieldSymbolDisplay = IsNullOrWhiteSpace(fieldSymbolDisplay) ? "NULL" : fieldSymbolDisplay;
            if (fieldSymbol?.IsStatic == true && fieldSymbol.IsReadOnly)
            {
                // Get the field type and name
                var fieldType = fieldSymbol.Type;
                var fieldName = fieldSymbol.Name;

                if (fieldType is INamedTypeSymbol ints && ints.TypeArguments.Length == 1 && ints.TypeArguments[0] is INamedTypeSymbol constFieldType && ValidConstTypeNames.Contains(constFieldType.Name))
                {
                    // create the const variable expresion
                    var funcResult = CompileAndRunFunc(context, compilation, constFieldType, fieldSymbol);
                    var constLiteralExpression =
                        SyntaxFactory.LiteralExpression(
                            ValidNumericConstTypeNames.Contains(constFieldType.Name) ? SyntaxKind.NumericLiteralExpression : SyntaxKind.StringLiteralExpression,
                            ValidNumericConstTypeNames.Contains(constFieldType.Name) ? SyntaxFactory.Token(default, SyntaxKind.NumericLiteralToken, funcResult.ToString(), funcResult.ToString(), default) :
                            SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, funcResult.ToString(), funcResult.ToString(), default));

                    // Create the const variable declaration
                    var constVariableDeclaration = SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.ParseTypeName(
                                constFieldType.ToDisplayString()
                                )
                            ).AddVariables(
                                SyntaxFactory.VariableDeclarator(fieldName)
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(constLiteralExpression)
                                    .NormalizeWhitespace()
                                )
                            )
                        )
                        .AddAttributeLists(GeneratedCodeAttributesList);
                    var partialTypeDeclaration =
                        SyntaxFactory.NamespaceDeclaration(
                            SyntaxFactory.ParseName(fieldSymbol.ContainingType.ContainingNamespace.ToDisplayString()))
                            .AddMembers(SyntaxFactory.TypeDeclaration(fieldDeclaration.Parent.Kind(), fieldSymbol.ContainingType.Name)
                            .WithKeyword(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                            .AddMembers(constVariableDeclaration));

                    // Add the class and const variable declarations to the compilation
                    context.AddSource($"{fieldName}.CompileTimeComputedField.g.cs",
                        HeaderTemplate.Render(new { Filename = $"{fieldName}.CompileTimeComputedField.g.cs" }) +
                        partialTypeDeclaration.NormalizeWhitespace().GetText().ToString());
                }
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CTCG002", "Error generatimg compile-time computed constant: must be const-able",
                    Format(CTCG002ErrorMessage, fieldSymbolDisplay),
                    "CTCG002: Field must be const-able", DiagnosticSeverity.Error, true),
                    fieldDeclaration.GetLocation()));
                }
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CTCG001", "Error generatimg compile-time computed constant: field must be static read-only",
                    Format(CTCG001ErrorMessage, fieldSymbolDisplay),
                    "CTCG001: Invalid field", DiagnosticSeverity.Error, true),
                    fieldDeclaration.GetLocation()));
            }
        }
    }

    private static object CompileAndRunFunc(SourceProductionContext context, Compilation compilation, INamedTypeSymbol returnType, IFieldSymbol fieldSymbol)
    {
        var programClassName = $"Program_{guid.NewGuid().ToString().Substring(0, 4)}";
        var code = $@"
            using System;
            public class {programClassName}
            {{
                public static {returnType.ToDisplayString()} RunFunc()
                {{
                    return {fieldSymbol.ContainingType.ToDisplayString()}.{fieldSymbol.Name}.Compute();
                }}
            }}
        ";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);

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
            var methodInfo = programType.GetMethod("RunFunc");

            var result = methodInfo.Invoke(null, null);

            return result;
        }
        else
        {
            foreach (var diagnostic in assembly.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
        return "NO RESULT";
    }
}

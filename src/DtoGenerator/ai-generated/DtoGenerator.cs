using System.Reflection.Metadata.Ecma335;
using System.Net.Security;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace Dgmjr.DtoGenerator.AiGenerated;

[Generator]
public partial class DtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context => context.AddSource(GenerateDtoAttributeFilename, GenerateDtoAttributeDeclaration));

        var validDtoDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(GenerateDtoAttributeName,
            IsEligibleType,
            (context, _) => context).Collect();

        context.RegisterSourceOutput(validDtoDeclarations, GenerateDtos);
    }

    private static bool IsPartialType(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax cls && cls.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PartialKeyword))
            || node is StructDeclarationSyntax @struct && @struct.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PartialKeyword))
            || node is InterfaceDeclarationSyntax @interface && @interface.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PartialKeyword));
    }

    private static bool IsEligibleType(SyntaxNode node, CancellationToken _ = default)
    {
        return IsPartialType(node) && node.GetAttribute(GenerateDtoAttributeName) != null;
    }

    private static void GenerateDtos(SourceProductionContext context, ImmutableArray<GeneratorAttributeSyntaxContext> generateDtoTypes)
    {
        // Generate code for each type
        foreach (var generateDtoType in generateDtoTypes)
        {
            var dtoAttribute = generateDtoType.TargetSymbol.GetAttributes().Single(attr => attr.AttributeClass.MetadataName.Equals(GenerateDtoAttributeName));
            var dtoType = dtoAttribute.NamedArguments.SingleOrDefault(arg => arg.Key == "dtoType").Value.Value as string ?? "Dto";
            var typeType = dtoAttribute.NamedArguments.SingleOrDefault(arg => arg.Key == "typeType").Value.Value as string ?? "RecordStruct";
            var typeName = dtoAttribute.NamedArguments.SingleOrDefault(arg => arg.Key == "typeName").Value.Value as string ?? $"{generateDtoType.TargetSymbol.Name}{dtoType}";
            var @namespace = dtoAttribute.NamedArguments.SingleOrDefault(arg => arg.Key == "@namespace").Value.Value as string ?? $"{generateDtoType.TargetSymbol.ContainingNamespace}.Dtos";

            if ((generateDtoType.TargetSymbol as INamedTypeSymbol)?.TypeKind is /*TypeKind.Interface or*/ TypeKind.Enum)
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("DTO001", "Invalid type", "GenerateDtoAttribute can only decorate classes and structs", "Code", DiagnosticSeverity.Error, true), Location.Create(generateDtoType.TargetNode.SyntaxTree, generateDtoType.TargetNode.Span)));
                continue;
            }

            var dtoTypeSymbol = GetTypeSymbol(compilation, dtoType);
            var typeTypeSymbol = GetTypeSymbol(compilation, typeType);

            var dtoProperties = generateDtoType.GetMembers().OfType<IPropertySymbol>()
                .Where(prop => prop.DeclaredAccessibility == Accessibility.Public && !prop.IsReadOnly && !prop.GetAttributes().Any(attr => attr.AttributeClass.Name == "IgnoreDtoPropertyAttribute" || attr.AttributeClass.Name == "KeyAttribute"))
                .Select(prop =>
                {
                    var isKey = prop.GetAttributes().Any(attr => attr.AttributeClass.Name == "KeyAttribute");
                    var databaseGeneratedAttribute = prop.GetAttributes().SingleOrDefault(attr => attr.AttributeClass.Name == "DatabaseGeneratedAttribute");
                    var hasDatabaseGeneratedAttribute = databaseGeneratedAttribute != null && databaseGeneratedAttribute.ConstructorArguments.Length == 1 && databaseGeneratedAttribute.ConstructorArguments[0].Value is int databaseGeneratedOption && databaseGeneratedOption != 0;
                    var ignore = isKey && hasDatabaseGeneratedAttribute;
                    return (prop, ignore);
                })
                .Where(tuple => !tuple.ignore)
                .Select(tuple => tuple.prop)
                .ToList();

            var dtoClass = SyntaxFactory.ClassDeclaration(typeName)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAttributeLists(SyntaxFactory.SingletonList(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.ParseName("System.Serializable"))))))
                .WithTypeParameterList(generateDtoType.TypeParameters.ToTypeParameterListSyntax())
                .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(dtoProperties.Select(prop =>
                {
                    var propType = prop.Type;
                    if (propType is INamedTypeSymbol namedType && namedType.IsGenericType && namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                    {
                        propType = namedType.TypeArguments.First();
                    }

                    var dtoProp = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propType.ToDisplayString()), prop.Name)
                        .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                        .WithAccessorList(SyntaxFactory.AccessorList(SyntaxFactory.List(new[]
                        {
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                        })));

                    return dtoProp;
                })));

            switch (typeTypeSymbol.SpecialType)
            {
                case SpecialType.System_Object:
                    dtoClass = dtoClass.WithKeyword(SyntaxFactory.Token(SyntaxKind.ClassKeyword));
                    break;
                case SpecialType.System_ValueType:
                    dtoClass = dtoClass.WithKeyword(SyntaxFactory.Token(SyntaxKind.StructKeyword));
                    break;
                case SpecialType.System_Enum:
                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("DTO002", "Invalid type", "GenerateDtoAttribute cannot generate DTOs for enums", "Code", DiagnosticSeverity.Error, true), generateDtoType.Locations.First()));
                    continue;
                default:
                    var isRecord = typeTypeSymbol.GetAttributes().Any(attr => attr.AttributeClass.Name == "System.Runtime.CompilerServices.IsRecordAttribute");
                    if (isRecord)
                    {
                        dtoClass = dtoClass.WithKeyword(SyntaxFactory.Token(SyntaxKind.RecordKeyword));
                    }
                    else
                    {
                        dtoClass = dtoClass.WithKeyword(SyntaxFactory.Token(SyntaxKind.ClassKeyword));
                    }
                    break;
            }

            var dtoNamespace = SyntaxFactory.ParseName(@namespace);
            var dtoNamespaceDeclaration = SyntaxFactory.NamespaceDeclaration(dtoNamespace)
                .WithUsings(SyntaxFactory.List(new[]
                {
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("AutoMapper")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(generateDtoType.ContainingNamespace.ToDisplayString()))
                }))
                .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(new[]
                {
                    dtoClass,
                    GenerateAutoMapperProfile(generateDtoType, dtoNamespace, dtoType, typeName)
                }));

            var dtoCompilationUnit = SyntaxFactory.CompilationUnit()
                .WithUsings(SyntaxFactory.List(new[]
                {
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Linq"))
                }))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(dtoNamespaceDeclaration));

            var sourceText = SourceText.From(dtoCompilationUnit.NormalizeWhitespace().ToFullString(), Encoding.UTF8);
            context.AddSource($"{typeName}.cs", sourceText);
        }
    }

    private static INamedTypeSymbol GetTypeSymbol(Compilation compilation, string typeName)
    {
        var typeSymbol = compilation.GetTypeByMetadataName(typeName);
        if (typeSymbol == null)
        {
            throw new InvalidOperationException($"Type {typeName} not found");
        }

        return typeSymbol;
    }

    private static ClassDeclarationSyntax GenerateAutoMapperProfile(INamedTypeSymbol sourceType, NameSyntax dtoNamespace, string dtoType, string dtoTypeName)
    {
        var profileClass = SyntaxFactory.ClassDeclaration($"{dtoTypeName}Profile")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
            .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("Profile")))))
            .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(new[]
            {
            GenerateAutoMapperMappings(sourceType, dtoNamespace, dtoType, dtoTypeName),
            GenerateAutoMapperReverseMappings(sourceType, dtoNamespace, dtoType, dtoTypeName)
            }));

        return profileClass;
    }

    private static MethodDeclarationSyntax GenerateAutoMapperMappings(INamedTypeSymbol sourceType, NameSyntax dtoNamespace, string dtoType, string dtoTypeName)
    {
        var sourceTypeSyntax = SyntaxFactory.ParseTypeName(sourceType.ToDisplayString());
        var dtoTypeSyntax = SyntaxFactory.ParseTypeName($"{dtoNamespace}.{dtoTypeName}, {dtoNamespace}");
        var mappingMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "ConfigureMappings")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(SyntaxFactory.Identifier("configuration"))
                .WithType(SyntaxFactory.ParseTypeName("IProfileExpression")))))
            .WithBody(SyntaxFactory.Block(
            SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("configuration"),
                    SyntaxFactory.IdentifierName("CreateMap")))
                .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                {
                    SyntaxFactory.Argument(sourceTypeSyntax),
                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                    SyntaxFactory.Argument(dtoTypeSyntax)
                }))))
            ));

        return mappingMethod;
    }

    private static MethodDeclarationSyntax GenerateAutoMapperReverseMappings(INamedTypeSymbol sourceType, NameSyntax dtoNamespace, string dtoType, string dtoTypeName)
    {
        var sourceTypeSyntax = SyntaxFactory.ParseTypeName(sourceType.ToDisplayString());
        var dtoTypeSyntax = SyntaxFactory.ParseTypeName($"{dtoNamespace}.{dtoTypeName}, {dtoNamespace}");
        var mappingMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "ConfigureReverseMappings")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(SyntaxFactory.Identifier("configuration"))
                .WithType(SyntaxFactory.ParseTypeName("IProfileExpression")))))
            .WithBody(SyntaxFactory.Block(
            SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("configuration"),
                    SyntaxFactory.IdentifierName("CreateMap")))
                .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                {
                    SyntaxFactory.Argument(dtoTypeSyntax),
                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                    SyntaxFactory.Argument(sourceTypeSyntax)
                }))))
            ));

        return mappingMethod;
    }
}

internal static class SyntaxNodeExtensions
{
    public static AttributeSyntax? GetAttribute(this SyntaxNode syntaxNode, string attributeMetadataName)
    {
        var attributeLists =
            (syntaxNode is BaseTypeDeclarationSyntax tds ? (SyntaxList<AttributeListSyntax>)tds.AttributeLists :
            (syntaxNode is BaseFieldDeclarationSyntax fds ? (SyntaxList<AttributeListSyntax>)fds.AttributeLists :
            (syntaxNode is BaseMethodDeclarationSyntax mds ? (SyntaxList<AttributeListSyntax>)mds.AttributeLists :
            (syntaxNode is BasePropertyDeclarationSyntax pds ? (SyntaxList<AttributeListSyntax>)pds.AttributeLists :
            SyntaxFactory.List<AttributeListSyntax>()))));
        return attributeLists.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name.ToString() == attributeMetadataName);
    }
}

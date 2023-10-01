namespace Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

public static class IMethodDeclarationSymbolExtensions
{
    /// <summary>
    /// We also need to convert the `IParameterSymbol`s to `ParameterSyntax`es.
    /// </summary>
    /// <param name="parameterSymbol">the <see cref="IParameterSymbol" /> to convert to a <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax" /></param>
    /// <returns>the <see cref="IParameterSymbol" /> as a <see cref="Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax" /></returns>
    public static ParameterSyntax ToParameterSyntax(this IParameterSymbol parameterSymbol)
    {
        return SyntaxFactory.Parameter(
            attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
            modifiers: SyntaxFactory.TokenList(),
            type: parameterSymbol.Type.ToTypeSyntax(),
            identifier: SyntaxFactory.Identifier(parameterSymbol.Name),
            @default: null
        );
    }

    /// <summary>
    /// also need to convert the <see cref="IMethodSymbol" />s to <see cref="MethodDeclarationSyntax" />es.
    /// </summary>
    /// <param name="methodSymbol">the <see cref="IMethodSymbol" />s to convert to a <see cref="MethodDeclarationSyntax" /></param>
    /// <returns>the <see cref="IMethodSymbol" />s as a <see cref="MethodDeclarationSyntax" /></returns>
    public static MethodDeclarationSyntax ToMethodDeclarationSyntax(this IMethodSymbol methodSymbol)
    {
        return SyntaxFactory.MethodDeclaration(
            attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
            modifiers: SyntaxFactory.TokenList(),
            returnType: methodSymbol.ReturnType.ToTypeSyntax(),
            explicitInterfaceSpecifier: null,
            identifier: SyntaxFactory.Identifier(methodSymbol.Name),
            typeParameterList: methodSymbol.TypeParameters.ToTypeParameterListSyntax(),
            parameterList: SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    methodSymbol.Parameters.Select(
                        parameterSymbol => parameterSymbol.ToParameterSyntax()
                    )
                )
            ),
            constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
            body: SyntaxFactory.Block(),
            expressionBody: null,
            semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)
        );
    }
}

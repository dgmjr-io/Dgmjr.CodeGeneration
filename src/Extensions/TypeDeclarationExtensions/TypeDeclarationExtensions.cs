namespace Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class TypeGenerationExtensions
{
    public static T WithMember<T>(this T t, MemberDeclarationSyntax member)
        where T : TypeDeclarationSyntax
    {
        return t.WithMembers(new[] { member });
    }

    public static T WithMembers<T>(this T t, IEnumerable<MemberDeclarationSyntax> members)
        where T : TypeDeclarationSyntax
    {
        return (t.WithMembers(SyntaxFactory.List(members)) as T)!;
    }

    public static T WithBaseType<T>(this T t, BaseTypeSyntax @base)
        where T : TypeDeclarationSyntax
    {
        return (t.AddBaseListTypes(new[] { @base }) as T)!;
    }

    public static T WithAttribute<T>(this T t, AttributeSyntax attribute)
        where T : TypeDeclarationSyntax
    {
        return (
            t.AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SeparatedList(
                        new[] { attribute },
                        new[] { SyntaxFactory.Token(SyntaxKind.CommaToken) }
                    )
                )
            ) as T
        )!;
    }

    public static T WithModifier<T>(this T t, SyntaxToken modifier)
        where T : TypeDeclarationSyntax
    {
        return (t.AddModifiers(new[] { modifier }) as T)!;
    }

    public static T WithModifier<T>(this T t, SyntaxKind modifier)
        where T : TypeDeclarationSyntax
    {
        return (t.WithModifier(SyntaxFactory.Token(modifier)) as T)!;
    }

    public static T WithKeyword<T>(this T t, SyntaxKind keyword)
        where T : TypeDeclarationSyntax
    {
        return (t.WithKeyword(SyntaxFactory.Token(keyword)) as T)!;
    }
}

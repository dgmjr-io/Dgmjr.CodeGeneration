/* 
 * ITypeSymbolExtensions.cs
 * 
 *   Created: 2023-06-05-05:49:41
 *   Modified: 2023-06-05-10:29:22
 * 
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *   
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */ 

//
// TypeParameterSymbolExtensions.cs
//
//   Created: 2022-10-31-01:35:49
//   Modified: 2022-11-04-11:14:46
//
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//


namespace Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;

public static partial class ITypeSymbolExtensions
{
    /// <summary>
    /// This function converts a collection of ITypeParameterSymbol objects into a TypeParameterListSyntax
    /// object.
    /// </summary>
    /// <param name="typeParameters">An IEnumerable of ITypeParameterSymbol objects representing the type
    /// parameters to be converted into a <see cref="TypeParameterListSyntax" /> object.</param>
    /// <returns>
    /// The method is returning the <paramref name="typeParameters" /> as a <see cref="TypeParameterListSyntax" /> object.
    /// </returns>
    public static TypeParameterListSyntax ToTypeParameterListSyntax(this IEnumerable<ITypeParameterSymbol> typeParameters)
    {
        return SyntaxFactory.TypeParameterList(
            SyntaxFactory.SeparatedList(
                typeParameters.Select(
                    typeParameter => SyntaxFactory.TypeParameter(typeParameter.Name))));
    }

    public static TypeParameterListSyntax ToTypeParameterListSyntax(this ImmutableArray<ITypeParameterSymbol> typeParameters)
        => typeParameters.AsEnumerable().ToTypeParameterListSyntax();

    /// <summary>
    /// We also need to convert the `ITypeSymbol`s to `TypeSyntax`es.
    /// </summary>
    /// <param name="typeSymbol">the <see cref="ITypeSymbol" /> to convert to a <see cref="TypeSyntax" /></param>
    /// <returns>the <see cref="ITypeSymbol" /> as a <see cref="TypeSyntax" /></returns>
    public static TypeSyntax ToTypeSyntax(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.TypeKind)
        {
            case TypeKind.Class:
            case TypeKind.Interface:
            case TypeKind.Enum:
            case TypeKind.Struct:
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;
                return SyntaxFactory.ParseTypeName(namedTypeSymbol.Name);

            case TypeKind.Array:
                var arrayTypeSymbol = (IArrayTypeSymbol)typeSymbol;
                return SyntaxFactory.ArrayType(arrayTypeSymbol.ElementType.ToTypeSyntax()).WithRankSpecifiers(SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier()));

            case TypeKind.Pointer:
                var pointerTypeSymbol = (IPointerTypeSymbol)typeSymbol;
                return SyntaxFactory.PointerType(pointerTypeSymbol.PointedAtType.ToTypeSyntax());

            case TypeKind.Dynamic:
                throw new InvalidOperationException("Can't generate code for type kind " + typeSymbol.TypeKind);

            case TypeKind.Delegate:
                var delegateTypeSymbol = (INamedTypeSymbol)typeSymbol;
                throw new InvalidOperationException("Can't generate code for type kind " + delegateTypeSymbol.TypeKind);

            default:
                throw new NotSupportedException();
        }
    }





    /// <summary>
    /// This function extends the functionality of the INamedTypeSymbol interface in C# by converting it
    /// to a ClassDeclarationSyntax.
    /// </summary>
    /// <param name="interfaceSymbol">The <see cref="INamedTypeSymbol" /> of the <see langword="interface" /> to generate a <see langword="class" /> for</param>
    public static ClassDeclarationSyntax ToClassDeclarationSyntax(this INamedTypeSymbol interfaceSymbol)
    {
        return SyntaxFactory.ClassDeclaration(
            attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
            modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
            identifier: SyntaxFactory.Identifier(interfaceSymbol.Name + "Impl"),
            typeParameterList: null,
            baseList: SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList(
                    new[] { SyntaxFactory.SimpleBaseType(interfaceSymbol.ToTypeSyntax()) as BaseTypeSyntax }
                )
            ),
            constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
            members: SyntaxFactory.List(
                interfaceSymbol.GetMembers().OfType<IMethodSymbol>().Select(
                    methodSymbol => methodSymbol.ToMethodDeclarationSyntax() as MemberDeclarationSyntax)
            )
        );
    }


    /// <summary>
    /// This function extends the functionality of the INamedTypeSymbol interface in C# by converting it
    /// to a CompilationUnitSyntax object.
    /// </summary>
    /// <param name="interfaceSymbol">An <see cref="INamedTypeSymbol" /> of an <see langword="interface" /> to generate a compilation unit for</param>
    public static CompilationUnitSyntax ToCompilationUnitSyntax(this INamedTypeSymbol interfaceSymbol)
    {
        return SyntaxFactory.CompilationUnit()
            .WithUsings(SyntaxFactory.List(new[]
            {
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Collections.Generic")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Linq")),
                SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Threading.Tasks"))
            }))
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                interfaceSymbol.ToClassDeclarationSyntax()));
    }
}

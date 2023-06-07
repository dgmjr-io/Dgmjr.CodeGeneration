//
// NamedTypeSymbolExtensions.cs
//
//   Created: 2022-10-30-10:35:49
//   Modified: 2022-10-30-10:40:59
//
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//

using System.Text;
using Microsoft.CodeAnalysis;

namespace Microsoft.CodeAnalysis;

internal static class NamedTypeSymbolExtensions
{
    public static string GenerateFileName(this INamedTypeSymbol namedTypeSymbol)
    {
        var typeName = namedTypeSymbol.GetFullType();
        return !(typeName.Contains('<') && typeName.Contains('>'))
            ? typeName
            : $"{typeName.Replace('.', '_').Replace('<', '_').Replace('>', '_').Replace(", ", "-")}_{typeName.Count(c => c == ',') + 1}";
    }

    public static string GetFullType(this INamedTypeSymbol namedTypeSymbol)
    {
        // https://www.codeproject.com/Articles/861548/Roslyn-Code-Analysis-in-Easy-Samples-Part
        //var str = new StringBuilder(namedTypeSymbol.Name);

        //if (namedTypeSymbol.TypeArguments.Count() > 0)
        //{
        //    str.AppendFormat("<{0}>", string.Join(", ", namedTypeSymbol.TypeArguments.OfType<INamedTypeSymbol>().Select(typeArg => typeArg.GetFullType())));
        //}

        return namedTypeSymbol.OriginalDefinition.ToString(); // str.ToString();
    }

    public static string ResolveClassNameWithOptionalTypeConstraints(
        this INamedTypeSymbol namedTypeSymbol,
        string className
    )
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            return className;
        }

        var str = new StringBuilder(
            $"{className}<{string.Join(", ", namedTypeSymbol.TypeArguments.Select(ta => ta.Name))}>"
        );

        foreach (
            var typeParameterSymbol in namedTypeSymbol.TypeArguments.OfType<ITypeParameterSymbol>()
        )
        {
            str.Append(typeParameterSymbol.GetWhereClause());
        }

        return str.ToString();
    }

    /// <summary>
    /// This C# extension method returns a string representing the "where" clause for a generic type.
    /// </summary>
    /// <param name="namedTypeSymbol">INamedTypeSymbol is an interface that represents a named type symbol
    /// in a .NET program. It provides information about a named type, such as its name, accessibility, base
    /// type, implemented interfaces, and type parameters.</param>
    /// <returns>
    /// If the `namedTypeSymbol` is not a generic type, an empty string is returned. Otherwise, the method
    /// iterates over the type arguments and appends the `where` constraints for each type parameter symbol
    /// using the `GetWhereStatement()` method. The resulting `where` statement is returned as a string.
    /// </returns>
    public static string GetWhereClause(this INamedTypeSymbol namedTypeSymbol)
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (
            var typeParameterSymbol in namedTypeSymbol.TypeArguments.OfType<ITypeParameterSymbol>()
        )
        {
            sb.Append(typeParameterSymbol.GetWhereClause());
        }

        return sb.ToString();
    }

    public static string GetWhereClause(this ITypeParameterSymbol typeParameterSymbol)
    {
        var sb = new StringBuilder();
        var constraints = typeParameterSymbol.ConstraintTypes;

        if (constraints.Length > 0)
        {
            sb.Append(" where ")
                .Append(typeParameterSymbol.Name)
                .Append(" : ")
                .Append(
                    string.Join(
                        ", ",
                        constraints
                            .Select(
                                c =>
                                    c.CanBeReferencedByName && c.Name != typeParameterSymbol.Name
                                        ? c.Name
                                        : null
                            )
                            .Concat(
                                new[]
                                {
                                    typeParameterSymbol.HasConstructorConstraint ? "new()" : null
                                }
                            )
                            .Concat(
                                new[]
                                {
                                    typeParameterSymbol.HasReferenceTypeConstraint ? "class" : null
                                }
                            )
                            .Concat(
                                new[]
                                {
                                    typeParameterSymbol.HasValueTypeConstraint ? "struct" : null
                                }
                            )
                            .Concat(
                                new[]
                                {
                                    typeParameterSymbol.HasNotNullConstraint ? "notnull" : null
                                }
                            )
                            .Concat(
                                new[]
                                {
                                    typeParameterSymbol.HasUnmanagedTypeConstraint
                                        ? "unmanaged"
                                        : null
                                }
                            )
                            .WhereNotNull()
                    )
                );
        }

        return sb.ToString();
    }

    /// <summary>
    /// See https://stackoverflow.com/questions/24157101/roslyns-gettypebymetadataname-and-generic-types
    /// </summary>
    public static string GenerateShortTypeName(
        this INamedTypeSymbol namedTypeSymbol,
        bool addBuilderPostFix = false
    )
    {
        var className = $"{namedTypeSymbol.Name}{(addBuilderPostFix ? "Builder" : string.Empty)}";
        var typeArguments = namedTypeSymbol.TypeArguments.Select(ta => ta.Name).ToArray();

        return !namedTypeSymbol.IsGenericType || typeArguments.Length == 0
            ? className
            : $"{className}<{string.Join(", ", typeArguments)}>";
    }

    public static string GenerateFullTypeName(
        this INamedTypeSymbol namedTypeSymbol,
        bool addBuilderPostFix = false
    )
    {
        return $"{namedTypeSymbol}{(addBuilderPostFix ? "Builder" : string.Empty)}";
    }
}

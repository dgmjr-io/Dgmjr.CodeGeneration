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
        return !(typeName.Contains('<') && typeName.Contains('>')) ?
            typeName :
            $"{typeName.Replace('.', '_').Replace('<', '_').Replace('>', '_').Replace(", ", "-")}_{typeName.Count(c => c == ',') + 1}";
    }

    public static string GetFullType(this INamedTypeSymbol namedTypeSymbol)
    {
        // https://www.codeproject.com/Articles/861548/Roslyn-Code-Analysis-in-Easy-Samples-Part
        //var str = new StringBuilder(namedTypeSymbol.Name);

        //if (namedTypeSymbol.TypeArguments.Count() > 0)
        //{
        //    str.AppendFormat("<{0}>", string.Join(", ", namedTypeSymbol.TypeArguments.OfType<INamedTypeSymbol>().Select(typeArg => typeArg.GetFullType())));
        //}

        return namedTypeSymbol.OriginalDefinition.ToString();// str.ToString();
    }

    public static string ResolveClassNameWithOptionalTypeConstraints(this INamedTypeSymbol namedTypeSymbol, string className)
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            return className;
        }

        var str = new StringBuilder($"{className}<{string.Join(", ", namedTypeSymbol.TypeArguments.Select(ta => ta.Name))}>");

        foreach (var typeParameterSymbol in namedTypeSymbol.TypeArguments.OfType<ITypeParameterSymbol>())
        {
            str.Append(typeParameterSymbol.GetWhereStatement());
        }

        return str.ToString();
    }

    public static string GetWhereStatement(this INamedTypeSymbol namedTypeSymbol)
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var typeParameterSymbol in namedTypeSymbol.TypeArguments.OfType<ITypeParameterSymbol>())
        {
            sb.Append(typeParameterSymbol.GetWhereStatement());
        }

        return sb.ToString();
    }

    /// <summary>
    /// See https://stackoverflow.com/questions/24157101/roslyns-gettypebymetadataname-and-generic-types
    /// </summary>
    public static string GenerateShortTypeName(this INamedTypeSymbol namedTypeSymbol, bool addBuilderPostFix = false)
    {
        var className = $"{namedTypeSymbol.Name}{(addBuilderPostFix ? "Builder" : string.Empty)}";
        var typeArguments = namedTypeSymbol.TypeArguments.Select(ta => ta.Name).ToArray();

        return !namedTypeSymbol.IsGenericType || typeArguments.Length == 0 ?
            className :
            $"{className}<{string.Join(", ", typeArguments)}>";
    }

    public static string GenerateFullTypeName(this INamedTypeSymbol namedTypeSymbol, bool addBuilderPostFix = false)
    {
        return $"{namedTypeSymbol}{(addBuilderPostFix ? "Builder" : string.Empty)}";
    }
}

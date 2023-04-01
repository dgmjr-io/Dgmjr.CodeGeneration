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

using Microsoft.CodeAnalysis;

namespace Microsoft.CodeAnalysis;

internal static class TypeParameterSymbolExtensions
{
    /// <summary>
    /// https://www.codeproject.com/Articles/871704/Roslyn-Code-Analysis-in-Easy-Samples-Part-2
    /// </summary>
    public static string GetWhereStatement(this ITypeParameterSymbol typeParameterSymbol)
    {
        var constraints = new List<string>();
        if (typeParameterSymbol.HasReferenceTypeConstraint)
        {
            constraints.Add("class");
        }

        if (typeParameterSymbol.HasValueTypeConstraint)
        {
            constraints.Add("struct");
        }

        if (typeParameterSymbol.HasConstructorConstraint)
        {
            constraints.Add("new()");
        }

        constraints.AddRange(typeParameterSymbol.ConstraintTypes.OfType<INamedTypeSymbol>().Select(constraintType => constraintType.GetFullType()));

        if (!constraints.Any())
        {
            return string.Empty;
        }

        return $" where {typeParameterSymbol.Name} : {string.Join(", ", constraints)}";
    }
}

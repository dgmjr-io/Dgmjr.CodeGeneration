//
// TypeSymbolExtensions.cs
//
//   Created: 2022-10-30-10:35:49
//   Modified: 2022-10-30-10:43:30
//
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//

using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;

namespace Microsoft.CodeAnalysis;

/// <summary>
/// Some extensions copied from:
/// - https://github.com/explorer14/SourceGenerators
/// - https://github.com/icsharpcode/RefactoringEssentials
/// </summary>
internal static class TypeSymbolExtensions
{
    // public static FluentTypeKind GetFluentTypeKind(this ITypeSymbol typeSymbol)
    // {
    //     if (typeSymbol.SpecialType == SpecialType.System_String)
    //     {
    //         return FluentTypeKind.String;
    //     }

    //     if (typeSymbol.TypeKind == TypeKind.Array)
    //     {
    //         return FluentTypeKind.Array;
    //     }

    //     if (typeSymbol.ImplementsInterfaceOrBaseClass(typeof(IDictionary<,>)) || typeSymbol.ImplementsInterfaceOrBaseClass(typeof(IDictionary)))
    //     {
    //         return FluentTypeKind.IDictionary;
    //     }

    //     if (typeSymbol.ImplementsInterfaceOrBaseClass(typeof(ReadOnlyCollection<>)))
    //     {
    //         return FluentTypeKind.ReadOnlyCollection;
    //     }

    //     if (typeSymbol.ImplementsInterfaceOrBaseClass(typeof(IList<>)) || typeSymbol.ImplementsInterfaceOrBaseClass(typeof(IList)))
    //     {
    //         return FluentTypeKind.IList;
    //     }

    //     if (typeSymbol.ImplementsInterfaceOrBaseClass(typeof(IReadOnlyCollection<>)))
    //     {
    //         return FluentTypeKind.IReadOnlyCollection;
    //     }

    //     if (typeSymbol.ImplementsInterfaceOrBaseClass(typeof(ICollection<>)) || typeSymbol.ImplementsInterfaceOrBaseClass(typeof(ICollection)))
    //     {
    //         return FluentTypeKind.ICollection;
    //     }

    //     if (typeSymbol.AllInterfaces.Any(i => i.SpecialType == SpecialType.System_Collections_IEnumerable))
    //     {
    //         return FluentTypeKind.IEnumerable;
    //     }

    //     return FluentTypeKind.Other;
    // }

    // https://stackoverflow.com/questions/39708316/roslyn-is-a-inamedtypesymbol-of-a-class-or-subclass-of-a-given-type
    public static bool ImplementsInterfaceOrBaseClass(this ITypeSymbol typeSymbol, Type typeToCheck)
    {
        return typeSymbol.ImplementsInterfaceOrBaseClass(typeToCheck.Name);
    }

    public static bool ImplementsInterfaceOrBaseClass(
        this ITypeSymbol typeSymbol,
        string typeToCheck
    )
    {
        if (typeSymbol == null)
        {
            return false;
        }

        if (typeSymbol.MetadataName == typeToCheck)
        {
            return true;
        }

        foreach (var @interface in typeSymbol.AllInterfaces)
        {
            if (@interface.MetadataName == typeToCheck)
            {
                return true;
            }
        }

        return typeSymbol.BaseType.ImplementsInterfaceOrBaseClass(typeToCheck);
    }

    public static bool CanSupportCollectionInitializer(this ITypeSymbol typeSymbol)
    {
        if (
            typeSymbol.AllInterfaces.Any(
                i => i.SpecialType == SpecialType.System_Collections_IEnumerable
            )
        )
        {
            var curType = typeSymbol;
            while (curType != null)
            {
                if (HasAddMethod(curType))
                {
                    return true;
                }

                curType = curType.BaseType;
            }
        }

        return false;
    }

    public static bool HasAddMethod(INamespaceOrTypeSymbol typeSymbol)
    {
        return typeSymbol
            .GetMembers(WellKnownMemberNames.CollectionInitializerAddMethodName)
            .OfType<IMethodSymbol>()
            .Any(m => m.Parameters.Any());
    }

    public static bool IsClass(this ITypeSymbol namedType) =>
        namedType.IsReferenceType && namedType.TypeKind == TypeKind.Class;

    public static bool IsStruct(this ITypeSymbol namedType) =>
        namedType.IsValueType && namedType.TypeKind == TypeKind.Struct;

    public static bool IsRecord(this ITypeSymbol namedType) =>
        namedType.IsValueType && namedType.TypeKind == TypeKind.Struct && namedType.IsRecord;
}

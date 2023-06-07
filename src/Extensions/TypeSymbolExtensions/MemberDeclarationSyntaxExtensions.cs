//
// MemberDeclarationSyntaxExtensions.cs
//
//   Created: 2022-11-10-06:59:34
//   Modified: 2022-11-10-07:00:36
//
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//
namespace Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class MemberDeclarationSyntaxExtensions
{
    public static AttributeSyntax? GetAttribute(
        this MemberDeclarationSyntax member,
        string attributeName
    )
    {
        return member.AttributeLists
            .SelectMany(a => a.Attributes)
            .FirstOrDefault(
                a =>
                    a.Name.ToString() == attributeName
                    || a.Name.ToString() == $"{attributeName}Attribute"
            );
    }
}

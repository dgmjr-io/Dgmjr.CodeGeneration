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
    public static AttributeSyntax? GetAttribute(this MemberDeclarationSyntax member, string attributeName)
    {
        return member.AttributeLists
            .SelectMany(a => a.Attributes)
            .FirstOrDefault(a => a.Name.ToString() == attributeName || a.Name.ToString() == $"{attributeName}Attribute");
    }

    public static ExpressionSyntax? GetParameterExpression(this AttributeSyntax attribute, string parameterName)
    {
        return attribute.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameEquals?.Name.ToString() == parameterName)
            ?.Expression;
    }

    public static ExpressionSyntax? GetParameterExpression(this AttributeSyntax attribute, int parameterIndex)
    {
        return attribute.ArgumentList.Arguments
            .ElementAtOrDefault(parameterIndex)
            ?.Expression;
    }

    public static string? GetParameterValue(this AttributeSyntax attribute, string parameterName)
    {
        return attribute.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameEquals?.Name.ToString() == parameterName)
            ?.Expression.GetText().ToString();
    }

    public static string? GetParameterValue(this AttributeSyntax attribute, int parameterIndex)
    {
        return attribute.ArgumentList.Arguments
            .ElementAtOrDefault(parameterIndex)
            ?.Expression.GetText().ToString();
    }

    public static object? GetParameterValue(this AttributeSyntax attribute, Compilation compilation, string parameterName)
    {
        var paramExp = attribute.GetParameterExpression(parameterName);
        if (paramExp is null)
            return null;
        else
        {
            var model = compilation.GetSemanticModel(paramExp.SyntaxTree);
            var constantValue = model.GetConstantValue(paramExp);
            return constantValue.HasValue ? constantValue.Value : null;
        }
    }

    public static object? GetParameterValue(this AttributeSyntax attribute, Compilation compilation, int parameterIndex)
    {
        var paramExp = attribute.GetParameterExpression(parameterIndex);
        if (paramExp is null)
            return null;
        else
        {
            var model = compilation.GetSemanticModel(paramExp.SyntaxTree);
            var constantValue = model.GetConstantValue(paramExp);
            return constantValue.HasValue ? constantValue.Value : null;
        }
    }
}

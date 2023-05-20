/*
 * AttributeSyntaxExtensions.cs
 *
 *   Created: 2023-05-03-03:46:55
 *   Modified: 2023-05-03-03:46:55
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Microsoft.CodeAnalysis;

using global::Microsoft.CodeAnalysis;
using global::System.Collections.Immutable;
using global::Microsoft.CodeAnalysis.CSharp.Syntax;
using global::Microsoft.CodeAnalysis.Text;

public static class AttributeSyntaxExtensions
{
    public static ExpressionSyntax? GetParameterExpression(
        this AttributeSyntax attribute,
        string parameterName
    )
    {
        return attribute.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameEquals?.Name.ToString() == parameterName)
            ?.Expression;
    }

    public static ExpressionSyntax? GetParameterExpression(
        this AttributeSyntax attribute,
        int parameterIndex
    )
    {
        return attribute.ArgumentList.Arguments.ElementAtOrDefault(parameterIndex)?.Expression;
    }

    public static string? GetParameterValue(this AttributeSyntax attribute, string parameterName)
    {
        return attribute.ArgumentList.Arguments
            .FirstOrDefault(a => a.NameEquals?.Name.ToString() == parameterName)
            ?.Expression.GetText()
            .ToString();
    }

    public static string? GetParameterValue(this AttributeSyntax attribute, int parameterIndex)
    {
        return attribute.ArgumentList.Arguments
            .ElementAtOrDefault(parameterIndex)
            ?.Expression.GetText()
            .ToString();
    }

    public static object? GetParameterValue(
        this AttributeSyntax attribute,
        Compilation compilation,
        string parameterName
    )
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

    public static object? GetParameterValue(
        this AttributeSyntax attribute,
        Compilation compilation,
        int parameterIndex
    )
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

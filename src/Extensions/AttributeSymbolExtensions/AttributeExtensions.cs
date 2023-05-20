//
// AttributeExtensions.cs
//
//   Created: 2022-11-10-06:43:47
//   Modified: 2022-11-10-06:43:47
//
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//
namespace Microsoft.CodeAnalysis;
using System.Text;

internal static class AttributeExtensions
{
    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeName)
    {
        return symbol?.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == attributeName);
    }

    public static object? GetAttributeArgumentValue(this AttributeData attribute, string argumentName)
    {
        var argument = attribute?.NamedArguments.FirstOrDefault(a => a.Key == argumentName);
        return argument?.Value.Value;
    }

    public static string? GetAttributeArgumentValueAsString(this AttributeData attribute, string argumentName)
    {
        var argument = attribute?.NamedArguments.FirstOrDefault(a => a.Key == argumentName);
        return argument?.Value.Value?.ToString();
    }

    public static object? GetAttributeArgumentValue(this AttributeData attribute, int argumentIndex)
    {
        var argument = attribute?.ConstructorArguments.ElementAtOrDefault(argumentIndex);
        return argument?.Value;
    }

    public static string? GetAttributeArgumentValueAsString(this AttributeData attribute, int argumentIndex)
    {
        var argument = attribute?.ConstructorArguments.ElementAtOrDefault(argumentIndex);
        return argument?.Value?.ToString();
    }

    public static string? GetAttributeArgumentValueAsString(this ISymbol symbol, string attributeName, string argumentName)
    {
        var attribute = symbol?.GetAttribute(attributeName);
        return attribute?.GetAttributeArgumentValueAsString(argumentName);
    }

    public static string? GetAttributeArgumentValueAsString(this ISymbol symbol, string attributeName, string argumentName, string defaultValue)
    {
        var attribute = symbol?.GetAttribute(attributeName);
        return attribute?.GetAttributeArgumentValueAsString(argumentName);
    }

    public static string GetAttributeText(this AttributeData attributeData)
    {
        var str = new StringBuilder($"[{attributeData.AttributeClass.Name}");

        if (attributeData.ConstructorArguments.Length > 0)
        {
            str.Append("(");

            foreach (var argument in attributeData.ConstructorArguments)
            {
                str.Append($"{argument.Value}, ");
            }

            str.Remove(str.Length - 2, 2);
            str.Append(")");
        }

        str.Append("]");

        return str.ToString();
    }

    public static string GetAttributeText(this AttributeData attributeData, string attributeName)
    {
        var str = new StringBuilder($"[{attributeName}");

        if (attributeData.ConstructorArguments.Length > 0)
        {
            str.Append("(");

            foreach (var argument in attributeData.ConstructorArguments)
            {
                str.Append($"{argument.Value}, ");
            }

            str.Remove(str.Length - 2, 2);
            str.Append(")");
        }

        str.Append("]");

        return str.ToString();
    }

    public static string GetAttributeText(this AttributeData attributeData, string attributeName, string attributeNamespace)
    {
        var str = new StringBuilder($"[{attributeNamespace}.{attributeName}");

        if (attributeData.ConstructorArguments.Length > 0)
        {
            str.Append("(");

            foreach (var argument in attributeData.ConstructorArguments)
            {
                str.Append($"{argument.Value}, ");
            }

            str.Remove(str.Length - 2, 2);
            str.Append(")");
        }

        str.Append("]");

        return str.ToString();
    }
}

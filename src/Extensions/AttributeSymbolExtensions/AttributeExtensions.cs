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
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class AttributeExtensions
{
    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeName)
    {
        return symbol?.GetAttributes().FirstOrDefault(a => a.AttributeClass.Name == attributeName);
    }

    public static object? GetAttributeConstructorArgument(this AttributeData attribute, string argumentName, object? @default = default)
    {
        var argument = attribute?.NamedArguments.FirstOrDefault(a => a.Key == argumentName);
        return argument?.Value.Value ?? @default;
    }

    /// <summary>
    /// This is a C# extension method that retrieves a constructor argument as a string from an
    /// attribute, with an optional default value.
    /// </summary>
    /// <param name="attribute">The <see cref="AttributeData" /> from which to retrieve the constructor argument</param>
    /// <param name="argumentName">The name of the constructor argument to retrieve as a string from the
    /// given attribute.</param>
    /// <param name="default">@default is an optional parameter with a default value of null. It is
    /// used to specify a default value to return if the constructor argument with the given name is not
    /// found in the attribute.</param>
    public static string? GetConstructorArgumentAsString(this AttributeData attribute, string argumentName, string? @default = default)
    {
        var argument = attribute?.NamedArguments.FirstOrDefault(a => a.Key == argumentName);
        return argument?.Value.Value?.ToString();
    }

    /// <summary>
    /// This is a C# extension method that retrieves the constructor argument at a specified index for a
    /// given attribute, with an optional default value.
    /// </summary>
    /// <param name="attribute">The <see cref="AttributeData" /> from which to retrieve the constructor argument</param>
    /// <param name="argumentIndex">The index of the constructor argument to retrieve from the
    /// attribute. The index starts at 0, so the first argument has an index of 0, the second argument
    /// has an index of 1, and so on.</param>
    /// <param name="default">@default is an optional parameter that specifies the default value to be
    /// returned if the constructor argument at the specified index is not found or cannot be converted
    /// to the expected type. If no default value is provided, the method will return null if the
    /// argument is not found or cannot be converted.</param>
    public static object? GetConstructorArgumentAtIndex(this AttributeData attribute, int argumentIndex, object? @default = default)
    {
        var argument = attribute?.ConstructorArguments.ElementAtOrDefault(argumentIndex);
        return argument?.Value;
    }

    /// <summary>
    /// This function is an extension method in C# that retrieves a constructor argument as a string
    /// from an attribute.
    /// </summary>
    /// <param name="attribute">AttributeData is a class in the System.Reflection namespace that
    /// represents the metadata of an attribute applied to a member. It contains information about the
    /// attribute's type, constructor arguments, and named arguments.</param>
    /// <param name="argumentIndex">The index of the constructor argument to retrieve as a string. This
    /// is used to get a specific argument value from the constructor of an attribute.</param>
    public static string? GetConstructorArgumentAsString(this AttributeData attribute, int argumentIndex)
    {
        var argument = attribute?.ConstructorArguments.ElementAtOrDefault(argumentIndex);
        return argument?.Value?.ToString();
    }

    public static string? GetAttributeConstructorArgumentAsString(this ISymbol symbol, string attributeName, string argumentName)
    {
        var attribute = symbol?.GetAttribute(attributeName);
        return attribute?.GetConstructorArgumentAsString(argumentName);
    }

    public static string? GetAttributeConstructorArgumentAsString(this ISymbol symbol, string attributeName, string argumentName, string defaultValue)
    {
        var attribute = symbol?.GetAttribute(attributeName);
        return attribute?.GetConstructorArgumentAsString(argumentName) ?? defaultValue;
    }

    public static string GetAttributeDeclarationAsCSharpCode(this AttributeData attributeData)
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

    public static ExpressionSyntax GetAttributeDeclarationSyntax(this AttributeData attributeData, string attributeName)
    {
        // Check if the attribute name matches the specified name
        if (attributeData.AttributeClass?.Name != attributeName)
        {
            return null;
        }

        // Create a syntax node for the attribute
        var attributeSyntax = SyntaxFactory.Attribute(
            SyntaxFactory.IdentifierName(attributeName),
            SyntaxFactory.AttributeArgumentList());

        // Add any arguments to the attribute syntax
        foreach (var argument in attributeData.NamedArguments)
        {
            var identifier = SyntaxFactory.IdentifierName(argument.Key);
            var value = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(argument.Value.Value.ToString()));
            var argumentSyntax = SyntaxFactory.AttributeArgument(identifier, null, value);
            attributeSyntax = attributeSyntax.AddArgumentListArguments(argumentSyntax);
        }

        // Return the attribute syntax as an expression
        return SyntaxFactory.ParseExpression(attributeSyntax.ToString());
    }

    public static string GetAttributeDeclarationAsCSharpCode(this AttributeData attributeData, string attributeName)
    {
        var attributeTypeName = attributeData.AttributeClass.ToDisplayString(NullableFlowState.MaybeNull, SymbolDisplayFormat.FullyQualifiedFormat);
        var arguments = string.Join(", ", attributeData.NamedArguments.Select(na => $"{na.Key} = {GetArgumentValueAsCode(na.Value)}"));
        if (attributeData.ConstructorArguments.Length > 0)
        {
            var positionalArguments = string.Join(", ", attributeData.ConstructorArguments.Select(GetArgumentValueAsCode));
            if (!string.IsNullOrEmpty(arguments))
                arguments += ", ";
            arguments += positionalArguments;
        }
        return $"[{attributeTypeName}({arguments})]";
    }

    private static string GetArgumentValueAsCode(TypedConstant argument)
    {
        var value = argument.Value;
        if (value == null)
        {
            return "null";
        }
        else if (argument.Kind == TypedConstantKind.Type)
        {
            return $"typeof({((ITypeSymbol)value).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)})";
        }
        else if (argument.Kind == TypedConstantKind.Enum)
        {
            return $"{((INamedTypeSymbol)argument.Type).EnumUnderlyingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{value}";
        }
        else if (argument.Kind == TypedConstantKind.Array)
        {
            var elementValues = string.Join(", ", argument.Values.Select(v => GetArgumentValueAsCode(v)));
            return $"new {argument.Type}[{argument.Values.Length}] {{ {elementValues} }}";
        }
        else
        {
            return value.ToString();
        }
    }
}

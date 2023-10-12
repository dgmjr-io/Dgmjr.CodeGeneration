/*
 * RegexGuardGenerator.cs
 *
 *   Created: 2023-06-28-09:38:52
 *   Modified: 2023-06-28-09:38:52
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;

[Generator]
public class RegexGuardGenerator : ISourceGenerator
{
    private const string AttributeText =
        @"
        using System;

        [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
        public class RegexGuardAttribute : Attribute
        {
            private readonly string _pattern;
            private readonly System.Text.RegularExpressions.Regex _regex;

            public RegexGuardAttribute(string pattern)
            {
                if (IsNullOrEmpty(pattern))
                    throw new ArgumentException(""Pattern cannot be null or empty."", nameof(pattern));

                _pattern = pattern;
                _regex = new System.Text.RegularExpressions.Regex(_pattern);
            }

            public void Validate(object value)
            {
                if (value == null)
                    return;

                if (!_regex.IsMatch(value.ToString()))
                    throw new ArgumentException($""Value does not match the specified pattern: {_pattern}"");
            }
        }";

    public void Initialize(InitializationContext context) { }

    public void Execute(SourceGeneratorContext context)
    {
        context.AddSource("RegexGuardAttribute.cs", SourceText.From(AttributeText, Encoding.UTF8));

        var compilation = context.Compilation;

        var attributeSymbol = compilation.GetTypeByMetadataName("RegexGuardAttribute");
        if (attributeSymbol == null)
            return;

        var syntaxReceiver = new SyntaxReceiver();
        compilation.SyntaxTrees
            .Select(s => s.GetRoot())
            .SelectMany(r => r.DescendantNodes())
            .OfType<VariableDeclarationSyntax>()
            .ToList()
            .ForEach(
                v =>
                    v.Variables
                        .ToList()
                        .ForEach(variable =>
                        {
                            var symbolInfo = compilation
                                .GetSemanticModel(v.SyntaxTree)
                                .GetSymbolInfo(variable);
                            var symbol =
                                symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();

                            if (
                                symbol != null
                                && symbol
                                    .GetAttributes()
                                    .Any(ad => ad.AttributeClass.Equals(attributeSymbol))
                            )
                                syntaxReceiver.Variables.Add(variable);
                        })
            );

        foreach (var variable in syntaxReceiver.Variables.Distinct())
        {
            var model = compilation.GetSemanticModel(variable.SyntaxTree);

            foreach (
                var declarator in variable.DescendantNodes().OfType<VariableDeclaratorSyntax>()
            )
            {
                var symbolInfo =
                    model.GetDeclaredSymbol(declarator) as IFieldSymbol
                    ?? model.GetDeclaredSymbol(declarator) as IPropertySymbol
                    ?? model.GetDeclaredSymbol(declarator) as IParameterSymbol;

                if (
                    symbolInfo != null
                    && symbolInfo
                        .GetAttributes()
                        .Any(ad => ad.AttributeClass.Equals(attributeSymbol))
                )
                {
                    var attributeData = symbolInfo
                        .GetAttributes()
                        .First(ad => ad.AttributeClass.Equals(attributeSymbol));
                    var regexPatternArg = attributeData.ConstructorArguments.First();

                    var sourceBuilder = new IndentedStringBuilder();
                    sourceBuilder.AppendLine(
                        $"{variable.Identifier.ValueText} {declarator.Identifier.ValueText};"
                    );
                    sourceBuilder.AppendLine(
                        $"public {variable.Type} {declarator.Identifier.ValueText}"
                    );
                    sourceBuilder.AppendLine("{");
                    sourceBuilder.IncrementIndent();
                    sourceBuilder.AppendLine("get");
                    sourceBuilder.AppendLine("{");
                    sourceBuilder.IncrementIndent();
                    sourceBuilder.AppendLine($"return {declarator.Identifier.ValueText};");
                    sourceBuilder.DecrementIndent();
                    sourceBuilder.AppendLine("}");
                    sourceBuilder.AppendLine($"set");
                    sourceBuilder.AppendLine("{");
                    sourceBuilder.IncrementIndent();

                    sourceBuilder.AppendLine(
                        $"({attributeData.AttributeClass.Name})attributeData.Constructor.Invoke(new object[] {{ \"{regexPatternArg.Value}\" }});"
                    );

                    sourceBuilder.AppendLine(
                        $"{attributeData.AttributeClass.Name}.Validate(value);"
                    );

                    sourceBuilder.AppendLine($"{declarator.Identifier.ValueText} = value;");

                    sourceBuilder.DecrementIndent();
                    sourceBuilder.AppendLine("}");
                    sourceBuilder.DecrementIndent();
                    sourceBuilder.Append("}");

                    context.AddSource(
                        $"{symbolInfo.Name}_guard.cs",
                        SourceText.From(sourceBuilder.ToString(), Encoding.UTF8)
                    );
                }
            }
        }
    }

    private class SyntaxReceiver : ISyntaxReceiver
    {
        public List<VariableDeclarationSyntax> Variables { get; } =
            new List<VariableDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (
                syntaxNode is VariableDeclarationSyntax variableDeclarationSyntax
                && variableDeclarationSyntax.Parent is FieldDeclarationSyntax fieldDeclarationSyntax
                && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
            )
            {
                return; // Ignore read-only fields.
            }

            if (
                !(
                    syntaxNode is VariableDeclaratorSyntax
                    || syntaxNode is ParameterSyntax
                    || syntaxNode is PropertyDeclarationSyntax
                )
            )
                return;

            this.Variables.Add(variableDeclarationSyntax);
        }
    }
}

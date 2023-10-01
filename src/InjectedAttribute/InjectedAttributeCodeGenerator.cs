/*
 * InjectedAttributeCodeGenerator.cs
 *
 *   Created: 2023-06-14-11:11:06
 *   Modified: 2023-06-14-11:11:06
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.CodeGeneration.InjectedAttribute;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Generator]
public class InjectedConstructorGenerator : IIncrementalGenerator
{
    private const string AttributeDeclaration =
        @$"  
            [System.AttributeUsage(System.AttributeTargets.Property)]  
            [System.CodeDom.Compiler.GeneratedCode(""InjectedConstructorGenerator"", ""{ThisAssembly.Info.Version}"")]
            public class InjectedAttribute : System.Attribute {{  }}  ";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the injected attribute
        context.RegisterPostInitializationOutput(
            (i) =>
                i.AddSource(
                    "InjectedAttribute.g.cs",
                    SourceText.From(AttributeDeclaration, Encoding.UTF8)
                )
        );

        // Register the syntax receiver
        context.RegisterForSyntaxNotifications(() => new InjectedSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Get the syntax receiver
        if (!(context.SyntaxReceiver is InjectedSyntaxReceiver receiver))
            return;

        // Get the compilation
        if (!(context.Compilation is CSharpCompilation compilation))
            return;

        // Get the list of properties with injected attributes
        var properties = receiver.PropertiesWithInjectedAttribute
            .Select(
                s =>
                    compilation.GetSemanticModel(s.SyntaxTree).GetDeclaredSymbol(s)
                    as IPropertySymbol
            )
            .Where(p => p != null);

        // Generate the code for partial classes with injected constructors
        var code = new StringBuilder();
        foreach (var property in properties)
        {
            code.AppendFormat(
                "public partial class {0} {{ ",
                property.ContainingType.ToDisplayString()
            );

            // Generate the injected constructor
            var parameterNames = new List<string>();
            var parameterTypes = new List<string>();
            foreach (var attribute in property.GetAttributes())
            {
                if (attribute.AttributeClass.ToDisplayString() != "InjectedAttribute")
                    continue;

                var paramName = attribute.ConstructorArguments.First().Value.ToString();
                var paramType = property.Type.ToDisplayString();

                parameterNames.Add(paramName);
                parameterTypes.Add(paramType);
            }

            code.AppendFormat(
                "public {0}({1}) {{ ",
                property.ContainingType.ToDisplayString(),
                string.Join(", ", parameterTypes.Zip(parameterNames, (t, n) => $"{t} {n}"))
            );
            foreach (var paramName in parameterNames)
            {
                code.AppendFormat("{0} = {1}; ", property.Name, paramName);
            }
            code.Append("}} ");

            // Generate overloads for existing constructors
            foreach (var ctor in property.ContainingType.Constructors)
            {
                var parameters = ctor.Parameters.Select(p => $"{p.Type} {p.Name}");
                var existingParameters = parameters.Any()
                    ? ", " + string.Join(", ", parameters)
                    : "";
                var overloadParameters = string.Join(
                    ", ",
                    parameterTypes.Zip(parameterNames, (t, n) => $"{t} {n}")
                );

                code.AppendFormat(
                    "public {0}({1}{2}) : this({3}) {{ }} ",
                    property.ContainingType.ToDisplayString(),
                    string.Join(", ", parameters),
                    existingParameters,
                    overloadParameters
                );
            }

            code.Append("}} ");
        }

        // Add the generated source code to the context
        context.AddSource(
            "InjectedConstructorGenerator.cs",
            SourceText.From(code.ToString(), Encoding.UTF8)
        );
    }
}

/*
 * MethodSymbolExtenisons.cs
 *
 *   Created: 2023-08-20-03:34:05
 *   Modified: 2023-08-20-03:34:05
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Emit;

public static class MemberSymbolInvocationExtensions
{
    public static object InvokeStaticMethod(this IMethodSymbol methodSymbol, Compilation compilation, params object[] arguments)
    {
        if (!methodSymbol.IsStatic || methodSymbol.DeclaredAccessibility != Accessibility.Public)
        {
            throw new InvalidOperationException($"The method {methodSymbol.ToDisplayString()} must be public and static.");
        }
        var programClassName = $"Program_{guid.NewGuid().ToByteArray().ToHexString()}";
        var program =
        $$"""
        using System;

        public static class {{programClassName}}
        {
            public static object Run() => {{methodSymbol.ContainingType.ToDisplayString()}}.{{methodSymbol.Name}}({{Join(", ", arguments.Select(arg => $"{(arg is string ? "\"" : "")}{arg}{(arg is string ? "\"" : "")}"))}});
        }
        """;
        return AddToCompilatonAndCallRun(compilation, program, programClassName);
    }

    public static object GetStaticPropertyValue(this IPropertySymbol propertySymbol, Compilation compilation)
    {
        if (!propertySymbol.IsStatic || propertySymbol.DeclaredAccessibility != Accessibility.Public)
        {
            throw new InvalidOperationException($"The property {propertySymbol.ToDisplayString()} must be public and static.");
        }
        var programClassName = $"Program_{guid.NewGuid().ToByteArray().ToHexString()}";
        var program =
        $$"""
        using System;

        public static class {{programClassName}}
        {
            public static object Run() => {{propertySymbol.ContainingType.ToDisplayString()}}.{{propertySymbol.Name}};
        }
        """;
        return AddToCompilatonAndCallRun(compilation, program, programClassName);
    }

    private static object AddToCompilatonAndCallRun(Compilation compilation, string cSharpCode, string programClassName)
    {
        compilation = compilation.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(cSharpCode, compilation.SyntaxTrees.First().Options));
        return EmitAndCallRun(compilation, programClassName, cSharpCode);
    }

    private static object EmitAndCallRun(Compilation compilation, string programClassName, string? extraInfo = "")
    {
        using var asmStream = new MemoryStream();
        var emitResult = compilation.Emit(asmStream, options: new EmitOptions(false, debugInformationFormat: DebugInformationFormat.Embedded));
        if (!emitResult.Success)
        {
            var errorDiagnostic = emitResult.Diagnostics.FirstOrDefault(diag => diag.Severity == DiagnosticSeverity.Error);
            throw new CompilationException($"{programClassName}: {errorDiagnostic?.GetMessage()}{(!IsNullOrEmpty(extraInfo) ? ", *" : "")}{extraInfo?.Replace("\n", " ").Replace("\r", " ")}{(!IsNullOrEmpty(extraInfo) ? "*" : "")}");
        }
        asmStream.Flush();
        var asm = Assembly.Load(asmStream.GetBuffer());
        var programClass = Find(asm.GetExportedTypes(), t => t.Name == programClassName);
        var runMethod = programClass.GetMethod("Run");
        return runMethod.Invoke(null, null);
    }
}

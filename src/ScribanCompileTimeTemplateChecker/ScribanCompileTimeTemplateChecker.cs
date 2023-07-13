namespace Dgmjr.CodeAnalysis.ScribanCompileTimeTemplateChecker;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ScribanCompileTimeTemplateChecker : DiagnosticAnalyzer
{
    private const string DiagnosticId = "SCRIB001";
    private const string Title = "Scriban Compile-Time Template Checker";
    private const string MessageFormat = "Error compiling the template: {0}";
    private const string Description = "Error compiling the template: {0}.";
    private const string Category = "Scriban";

    private static readonly DiagnosticDescriptor Rules = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: false, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rules);

    public override void Initialize(AnalysisContext context)
    {
        throw new NotImplementedException();
    }
}

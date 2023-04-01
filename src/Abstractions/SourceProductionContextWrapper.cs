/* 
 * SourceProductionContextWrapper.cs
 * 
 *   Created: 2022-11-02-12:41:39
 *   Modified: 2022-11-12-12:20:09
 * 
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *   
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.CodeGeneration.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
public class SourceProductionContextWrapper : IGeneratorContextWrapper
{
    private readonly SourceProductionContext _context;
    public SourceProductionContextWrapper(SourceProductionContext context)
    {
        _context = context;
    }

    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        _context.ReportDiagnostic(diagnostic);
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Location> additionalLocations, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, additionalLocations, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Location> additionalLocations, IReadOnlyCollection<Diagnostic> relatedDiagnostics, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, additionalLocations, relatedDiagnostics, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Location> additionalLocations, IReadOnlyCollection<Diagnostic> relatedDiagnostics, bool isSuppressed, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, additionalLocations, relatedDiagnostics, isSuppressed, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Location> additionalLocations, IReadOnlyCollection<Diagnostic> relatedDiagnostics, bool isSuppressed, int warningLevel, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, additionalLocations, relatedDiagnostics, isSuppressed, warningLevel, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Location> additionalLocations, IReadOnlyCollection<Diagnostic> relatedDiagnostics, bool isSuppressed, int warningLevel, bool isError, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, additionalLocations, relatedDiagnostics, isSuppressed, warningLevel, isError, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Location> additionalLocations, IReadOnlyCollection<Diagnostic> relatedDiagnostics, bool isSuppressed, int warningLevel, bool isError, IReadOnlyCollection<AdditionalText> additionalFiles, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, additionalLocations, relatedDiagnostics, isSuppressed, warningLevel, isError, additionalFiles, args));
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, IReadOnlyDictionary<string, string> properties, IReadOnlyCollection<Location> additionalLocations, IReadOnlyCollection<Diagnostic> relatedDiagnostics, bool isSuppressed, int warningLevel, bool isError, IReadOnlyCollection<AdditionalText> additionalFiles, IReadOnlyCollection<Diagnostic> suppressedDiagnostics, params object[] args)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, properties, additionalLocations, relatedDiagnostics, isSuppressed, warningLevel, isError, additionalFiles, suppressedDiagnostics, args));
    }
    public void AddSource(string fileName, string source)
    {
        _context.AddSource(fileName, source);
    }
    public void AddSource(string fileName, SourceText source)
    {
        _context.AddSource(fileName, source);
    }
}

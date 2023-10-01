using System.Diagnostics;

/*
 * DiagnosticsExtensions.cs
 *
 *   Created: 2023-09-02-04:00:14
 *   Modified: 2023-09-02-04:00:15
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Microsoft.CodeAnalysis;

public static class DiagnosticsExtensions
{
    public static IEnumerable<Diagnostic> Errors(this IEnumerable<Diagnostic> diagnostics) =>
        diagnostics.Where(diag => diag.Severity == DiagnosticSeverity.Error);
}

/*
 * IncrementalGeneratorInitializationContextExtensions.cs
 *
 *   Created: 2023-05-03-03:36:56
 *   Modified: 2023-05-03-03:36:56
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

public static class IncrementalGeneratorInitializationContextExtensions
{
    public static void RegisterPostInitializationOutput(
        this IncrementalGeneratorInitializationContext context,
        string hint,
        string text
    )
    {
        var sourceText = SourceText.From(text, Encoding.UTF8);
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(hint, sourceText));
    }

    public static void RegisterPostInitializationOutput(
        IncrementalGeneratorInitializationContext ctx,
        string hint,
        SourceText text
    )
    {
        ctx.RegisterPostInitializationOutput(ctx2 => ctx2.AddSource(hint, text));
    }
}

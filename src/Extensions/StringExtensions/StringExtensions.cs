/*
 * StringExtensions.cs
 *
 *   Created: 2023-05-03-03:52:36
 *   Modified: 2023-05-03-03:52:41
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */


using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.CodeAnalysis;

internal static class StringExtensions
{
    private const string GenericTypeArgumentPattern = @"(?<=\<)(.*?)(?=\>)";

#if NET7_0_OR_GREATER
    [CompiledRegex(GenericTypeArgumentPattern)]
    private static partial Regex ExtractValueBetweenRegex();
#else
    private static readonly Regex _extractValueBetween = new(GenericTypeArgumentPattern, Compiled);

    private static Regex ExtractValueBetweenRegex() => _extractValueBetween;
#endif

    public static bool TryGetGenericTypeArguments(
        this string input, /*[NotNullWhen(true)]*/
        out string? genericTypeArgumentValue
    )
    {
        genericTypeArgumentValue = null;

        var match = ExtractValueBetweenRegex().Match(input);

        if (match.Success)
        {
            genericTypeArgumentValue = match.Value;
            return true;
        }

        return false;
    }

    public static string TrimFromSentinel(this string input, string sentinel)
    {
        var index = input.IndexOf(sentinel);

        if (index == -1)
        {
            return input;
        }

        return input.Substring(index + sentinel.Length).Trim();
    }

    public static string TrimToSentinel(this string input, string sentinel)
    {
        var index = input.IndexOf(sentinel);

        if (index == -1)
        {
            return input;
        }

        return input.Substring(0, index).Trim();
    }
}

/*
 * StringExtensions.cs
 *
 *   Created: 2022-10-31-01:35:49
 *   Modified: 2022-11-11-05:51:15
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Microsoft.CodeAnalysis;

internal static class StringExtensions
{
    private static readonly Regex ExtractValueBetween = new("(?<=<).*(?=>)", RegexOptions.Compiled);

    public static bool TryGetGenericTypeArguments(this string input, /*[NotNullWhen(true)]*/ out string? genericTypeArgumentValue)
    {
        genericTypeArgumentValue = null;

        var match = ExtractValueBetween.Match(input);

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

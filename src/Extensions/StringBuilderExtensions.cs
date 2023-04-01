//
// StringBuilderExtensions.cs
//
//   Created: 2022-10-30-10:35:49
//   Modified: 2022-10-30-10:42:27
//
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
//

// ReSharper disable once CheckNamespace
namespace System.Text;

internal static class StringBuilderExtensions
{
    public static void AppendLines(this StringBuilder sb, IEnumerable<string> values, string prefix = "")
    {
        sb.AppendLine(string.Join($"{prefix}\r\n", values));
    }
}

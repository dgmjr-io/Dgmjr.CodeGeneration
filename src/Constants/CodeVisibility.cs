/* 
 * CodeVisibility.cs
 * 
 *   Created: 2023-04-08-06:37:21
 *   Modified: 2023-04-08-06:37:21
 * 
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *   
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System;
using System.Reflection;

namespace Dgmjr.CodeGeneration;
public static partial class Constants
{
    public record struct CodeVisibility : IEquatable<CodeVisibility>
    {
        private CodeVisibility(string value) { Value = value; }

        public static readonly CodeVisibility Public = new (nameof(Public).ToLower());
        public static readonly CodeVisibility Private = new (nameof(Private).ToLower());
        public static readonly CodeVisibility Protected = new (nameof(Protected).ToLower());
        public static readonly CodeVisibility Internal = new (nameof(Internal).ToLower());
        public static readonly CodeVisibility ProtectedInternal = new ($"{nameof(Protected)} {nameof(Internal)}".ToLower());
        public static readonly CodeVisibility PrivateProtected = new ($"{nameof(Private)} {nameof(Protected)}".ToLower());

        public string Value { get; }
    }
}

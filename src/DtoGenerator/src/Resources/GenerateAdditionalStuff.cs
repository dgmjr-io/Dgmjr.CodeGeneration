/*
 * GenerateAdditionalStuff.cs
 *
 *   Created: 2023-01-03-05:46:04
 *   Modified: 2023-01-03-05:46:04
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */


[Flags]
public enum GenerateAdditionalStuff
{
    /// <summary>Don't generate any additional stuff</summary>
    None = 0,
    /// <summary>Generate an AutoMapper profile for this type</summary>
    AutoMapper = 1,
    /// <summary>Generate a JsonConverter for this type</summary>
    JsonConverter = 2,
    /// <summary>Generate an AutoMapper profile and a JsonConverter for this type</summary>
    All = AutoMapper | JsonConverter
}

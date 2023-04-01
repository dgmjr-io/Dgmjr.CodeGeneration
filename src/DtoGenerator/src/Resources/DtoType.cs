/*
 * DtoType.cs
 *
 *   Created: 2023-01-03-05:45:23
 *   Modified: 2023-01-03-05:45:23
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

[System.Flags]
internal enum DtoTypes
{
    /// <summary>Don't generate a DTO for this type</summary>
    None = 0,
    /// <summary>Generate a create DTO for this type</summary>
    Create = 1,
    /// <summary>Generate an update DTO for this type</summary>
    Update = 2,
    /// <summary>Generate a view DTO for this type</summary>
    View = 4,
    All = Create | Update | View
}

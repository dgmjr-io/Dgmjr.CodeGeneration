/*
 * IgnoreForDtoAttribute.cs
 *
 *   Created: 2023-01-03-07:05:10
 *   Modified: 2023-01-03-07:05:10
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
internal class IgnoreDtoPropertyAttribute : Attribute
{
    public IgnoreDtoPropertyAttribute() { }
}

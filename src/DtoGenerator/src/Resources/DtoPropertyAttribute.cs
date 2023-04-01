/*
 * DtoPropertyAttribute.cs
 *
 *   Created: 2023-01-03-05:52:04
 *   Modified: 2023-01-03-05:52:04
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
internal sealed class DtoPropertyAttribute : Attribute
{
    /// <summary>Decorate a property with this attribute to specify the name of the property in the DTO.</summary>
    /// <param name="dtoTypes">The DTO types for which the property should be generated.</param>
    /// <param name="name">The property's name in the DTO.  If none is specified, the same name as the parent type will be used.</param>
    /// <param name="isRequired">Whether the property will be decorated with a <see cref="RequiredAttribute" /></param>
    /// <param name="isReadOnly">Whether the property should be generated with only a <see langword="get" />ter</param>
    public DtoPropertyAttribute(DtoTypes dtoTypes = DtoTypes.All, string? name = null, bool isRequired = false, bool isReadOnly = false)
    {
    }
}

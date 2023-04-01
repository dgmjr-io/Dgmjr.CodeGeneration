/*
 * GenerateDtoAttribute.cs
 *
 *   Created: 2023-01-03-05:52:49
 *   Modified: 2023-01-03-05:52:49
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
internal sealed class GenerateDtoAttribute : Attribute
{
    /// <summary>Decorate a class or struct with this attribute to generate a DTO for it.</summary>
    /// <param name="type">The type(s) of DTO(s) to generate</param>
    /// <param name="dataStructureType">The type(s) of data structure(s) to generate</param>
    /// <param name="name" example="UserInsertDto">The DTO's name.  Defaults to "<c>{TypeName}{DtoType}Dto</c>"</param>
    /// <param name="namespace" example="Models.DTOs">The DTO's namespace (if different from the parent class' namespace)</param>
    public GenerateDtoAttribute(
        DtoTypes type = DtoTypes.All,
        DataStructureType dataStructureType = DataStructureType.RecordClass,
        string? name = null, string? @namespace = null,
        GenerateAdditionalStuff additionalStuff = GenerateAdditionalStuff.All)
    { }
}

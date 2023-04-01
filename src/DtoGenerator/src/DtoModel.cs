/*
 * DtoModel.cs
 *
 *   Created: 2023-01-03-07:34:00
 *   Modified: 2023-01-03-07:34:00
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

internal record struct DtoModel(DtoTypes DtoType)
{
    public string Header { get; set; }
    public string Filename { get; set; }
    public string Timestamp { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string[] Usings { get; set; }
    public string[] Attributes { get; set; }
    public string PropertiesVisibilityCode => DataStructureType switch
    {
        DataStructureType.Class => "public virtual",
        DataStructureType.Struct => "public",
        DataStructureType.RecordClass => "public virtual",
        DataStructureType.RecordStruct => "public",
        _ => throw new ArgumentOutOfRangeException(nameof(DtoType), DtoType, null)
    };
    public PropertyModel[] Properties { get; set; }
    public DataStructureType DataStructureType { get; set; }
    public string DataStructureTypeCode => DataStructureType switch
    {
        DataStructureType.Class => "class",
        DataStructureType.RecordClass => "record class",
        DataStructureType.Struct => "struct",
        DataStructureType.RecordStruct => "record struct",
        _ => throw new ArgumentOutOfRangeException(nameof(DataStructureType), DataStructureType, null)
    };
    public GenerateAdditionalStuff GenerateAdditionalStuff { get; set; }
}

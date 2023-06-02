/*
 * PropertyModel.cs
 *
 *   Created: 2023-01-03-07:16:02
 *   Modified: 2023-01-03-07:16:02
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using Microsoft.CodeAnalysis;

internal record struct PropertyModel(ITypeSymbol property_type, string property_name, bool is_required = false)
{
    public string PropertyTypeCode => property_type.ToDisplayString();
public string PropertyName => property_name;
public bool IsRequired => is_required;
public string RequiredAttributeCode => IsRequired ? "[Required]" : string.Empty;
public string PropertyCode => $"{PropertyTypeCode} {PropertyName} {{ get; set; }}";
}

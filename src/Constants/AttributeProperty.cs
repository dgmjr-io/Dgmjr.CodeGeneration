/*
 * AttributeProperty.cs
 *
 *   Created: 2022-12-10-02:05:58
 *   Modified: 2022-12-10-02:05:58
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.CodeGeneration;

public record struct AttributeProperty(System.Type property_type, string property_name, string default_value = "null", bool is_required = false);

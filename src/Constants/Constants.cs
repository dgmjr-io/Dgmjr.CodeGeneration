/*
 * Constants.cstemplate
 *
 *   Created: 2022-10-31-07:30:55
 *   Modified: 2022-12-08-01:01:51
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

#pragma warning disable

namespace Dgmjr.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

public static partial class Constants
{
    public const string BeginCodeHeaderSentinel = "// BEGIN CODE HEADER";
    public const string AuthorsPlaceholder = "$AUTHORS";
    public const string AuthorNamePlaceholder = "$AUTHOR";
    public const string AuthorEmailPlaceholder = "$AUTHOR_EMAIL";
    public const string AuthorUrlPlaceholder = "$AUTHOR_URL";
    public const string LicensePlaceholder = "$LICENSE";
    public const string YearPlaceholder = "$YEAR";
    public const string TimestampPlaceholder = "$TIMESTAMP";
    public const string FilenamePlaceholder = "$FILENAME";
    public const string AttributeClassNamePlaceholder = "$ATTRIBUTE_CLASS_NAME";
    public const string AttributeTargetsPlaceholder = "$ATTRIBUTE_TARGETS";
    public const string AttributeParamsPlaceholder = "$PARAMS";
    public const string AttributePropertiesDeclarationsPlaceholder = "$PROPS";
    public const string AttributePropertyAssignmentsPlaceholder = "$ASSIGNMENTS";
    public const string PackageProjectUrl = nameof(PackageProjectUrl);
    public const string AuthorsPropertyName = "Authors";
    public const string PackageLicenseExpression = nameof(PackageLicenseExpression);
    public const string AuthorsBuildProperty = $"build_property.{AuthorsPropertyName}";
    public const string LicenseBuildProperty = $"build_property.{PackageLicenseExpression}";
    public const string ProjectUrlBuildProperty = $"build_property.{PackageProjectUrl}";

    public const string CodeHeaderTemplate =
    """"
    //
// <auto-generated>
//     This code was generated by a tool.
//     Do not modify this file directly.
// </auto-generated>
//
// $FILENAME
//
//   Generated: $TIMESTAMP
//
//   $AUTHORS
//      $AUTHOR_URL
//
//   Copyright © $YEAR $COPYRIGHT_OWNER, All Rights Reserved
//      License: $LICENSE (https://opensource.org/licenses/$LICENSE)
//
"""";

    public const string MultipleAuthorsCodeHeaderCommentTemplate =
""""
// Authors:

"""";

    public const string MultipleAuthorsCodeHeaderCommentTemplate_PerAuthor =
""""
//    $AUTHOR_NAME <$AUTHOR_EMAIL>

"""";
    public const string SingleAuthorCodeHeaderTemplateString =
""""
// Author: $AUTHOR_NAME <$AUTHOR_EMAIL>

"""";

    public static readonly CodeTemplate SingleAuthorCodeHeaderTemplate = new CodeTemplate(SingleAuthorCodeHeaderTemplateString);
    public const string AttributeDeclarationTemplateString =
""""
#nullable enable
    [AttributeUsage({{ regex.replace(regex.replace attribute_targets "(?:(?:^)|(?: ))" "System.AttributeTargets.") "," " | " }
}, AllowMultiple = false)]
internal class {{ attribute_class_name }} : { { attribute_base_type.full_name } }
{
    public
{ { attribute_class_name } } ({ { for p in attribute_properties } }
{ { p.property_type.full_name } }
{ { if !for.last } }, { { end } }
{ { end } })
    {
    { { ~ for p in attribute_properties ~} }
    this.{ { ~p.property_name } } = { { p.property_name } } ?? { { p.default_value } };
    { { ~end ~} }
}

{ { ~ for p in attribute_properties ~} }
public
{ { p.property_type.full_name } }
{ { p.property_name } }
{ get; set; }
{ { ~end ~} }
}

"""";
public static readonly Scriban.Template AttributeDeclarationTemplate = Scriban.Template.Parse(AttributeDeclarationTemplateString);


public static readonly string ToolName = typeof(Constants).Assembly.GetName().Name;
public static readonly string ToolVersion = typeof(Constants).Assembly.GetName().GetName().Version.ToString();
public static readonly string MinimalCodeHeaderTemplateString =
$$$""""
/*
*  <auto-generated>
*      This code was generated by the{{{ ToolName }}}, version {{{ ToolVersion }}}
*      Do not modify this file directly.
*  </auto-generated>
*
*  {{ filename }}
*
*    Generated: {{ timestamp }}
*       Generated by: {{{ System.Environment.UserName }}}
*/
"""";
public static readonly Scriban.Template MinimalCodeHeaderTemplate = Scriban.Template.Parse(MinimalCodeHeaderTemplateString);

public static string GenerateMininmalCodeHeader(string filename, string timestamp)
{
    return MinimalCodeHeaderTemplate.Render(new { filename, timestamp });
}

public static string GenerateAttributeDeclaration(string attributeName, AttributeTargets attributeTargets = AttributeTargets.All, Type baseType = default, params AttributeProperty[] properties)
{
    baseType ??= typeof(Attribute);
    var result =
    MinimalCodeHeaderTemplate.Render(new { filename = $"{attributeName}.cs", timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm") }) +
    Environment.NewLine +
    AttributeDeclarationTemplate.Render(new AttributeInfo
    (
        attributeName,
        baseType,
        attributeTargets.ToString(),
        properties
    ));

    return result;
}

public static string GenerateAttributeDeclaration(AttributeInfo attributeInfo)
{
    return MinimalCodeHeaderTemplate.Render(attributeInfo);
}

public static string TrimToSentinel(this string codeHeader)
{
    var sentinelIndex = codeHeader.IndexOf(BeginCodeHeaderSentinel);
    if (sentinelIndex == -1)
    {
        return codeHeader;
    }
    return codeHeader.Substring(sentinelIndex + BeginCodeHeaderSentinel.Length).Trim();
}

public static IEnumerable<(string Name, string? Email)> ParseAuthors(string authors)
{
    var authorsList = authors.Split(';');
    foreach (var author in authorsList)
    {
        var authorParts = author.Split('<');
        var authorName = authorParts.First().Trim();
        var authorEmail = authorParts.Skip(1).FirstOrDefault()?.Trim().TrimEnd('>');
        yield return (authorName, authorEmail);
    }
}
}

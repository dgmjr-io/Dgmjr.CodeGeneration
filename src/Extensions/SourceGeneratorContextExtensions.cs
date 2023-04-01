// 
// SourceGeneratorContextExtensions.cs
// 
//   Created: 2022-11-04-10:32:12
//   Modified: 2022-11-04-11:17:06
// 
//   Author: David G. Moore, Jr. <david@dgmjr.io>
//   
//   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
//      License: MIT (https://opensource.org/licenses/MIT)
// 

// // 
// // SourceGeneratorContextExtensions.cs
// // 
// //   Created: 2022-11-04-10:32:12
// //   Modified: 2022-11-04-10:39:06
// // 
// //   Author: David G. Moore, Jr. <david@dgmjr.io>
// //   
// //   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
// //      License: MIT (https://opensource.org/licenses/MIT)
// // 

// using System.Text;
// using Microsoft.CodeAnalysis;

// namespace Microsoft.CodeAnalysis;

// public static class SourceGeneratorContextExtensions
// {
//     private const string SourceItemGroupMetadata = "build_metadata.AdditionalFiles.SourceItemGroup";

//     public static string GetMSBuildProperty(
//         this Microsoft.CodeAnalysis.SourceGeneratorContext context,
//         string name,
//         string defaultValue = "")
//     {
//         context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out var value);
//         return value ?? defaultValue;
//     }

//     public static string[] GetMSBuildItems(this SourceGeneratorContext context, string name)
//         => context
//             .AdditionalFiles
//             .Where(f => context.AnalyzerConfigOptions
//                 .GetOptions(f)
//                 .TryGetValue(SourceItemGroupMetadata, out var sourceItemGroup)
//                 && sourceItemGroup == name)
//             .Select(f => f.Path)
//             .ToArray();
// }

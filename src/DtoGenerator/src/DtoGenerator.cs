/*
 * DtoGenerator.cs
 *
 *   Created: 2022-12-06-07:42:23
 *   Modified: 2022-12-06-07:42:24
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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class DtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(Constants.GenerateDtoAttributeMetadataName,
            (node, _) => node is TypeDeclarationSyntax typeDeclarationSyntax &&
                        typeDeclarationSyntax.AttributeLists.Any(attr => attr.Attributes.Any(attr => attr.Name.ToString() == Constants.GenerateDtoAttributeMetadataName)),
            (ctx, _) => (ctx.Attributes, ctx.TargetNode as TypeDeclarationSyntax, ctx.TargetSymbol as ITypeSymbol, ctx.SemanticModel));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(Constants.DtoTypeMetadataName + ".cs", Constants.DtoTypeDeclaration));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(Constants.DataStructureTypeMetadataName + ".cs", Constants.DataStructureTypeDeclaration));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(Constants.GenerateDtoAttributeMetadataName + ".cs", Constants.GenerateDtoAttributeDeclaration));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(Constants.GenerateAdditionalStuffMetadataName + ".cs", Constants.GenerateAdditionalStuffDeclaration));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(Constants.DtoPropertyAttributeMetadataName + ".cs", Constants.DtoPropertyAttributeDeclaration));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(Constants.IgnoreDtoPropertyAttributeMetadataName + ".cs", Constants.IgnoreDtoPropertyAttributeDeclaration));
    }

    public virtual void GenerateDtos(SourceProductionContext context, ImmutableArray<(AttributeData AttributeData, TypeDeclarationSyntax SyntaxNode, ITypeSymbol Symbol, SemanticModel SemanticModel)> typeDeclarations)
    {
        foreach (var typeDeclaration in typeDeclarations)
        {
            var (attributeData, syntaxNode, symbol, semanticModel) = typeDeclaration;
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var dtoNamespace = namespaceName + ".Dtos";

            var autoMapperMappings = new List<string>();

            var dtoTypes = attributeData.ConstructorArguments.FirstOrDefault().Value as DtoTypes? ?? DtoTypes.None;
            if (dtoTypes == DtoTypes.None)
            {
                continue;
            }
            var dtoModels = GeneratePreliminaryDtoModels(dtoTypes).ToDictionary(dto => dto.DtoType, dto => dto);

            IEnumerable<DtoModel> GeneratePreliminaryDtoModels(DtoTypes dtoTypes)
            {
                if (dtoTypes.HasFlag(DtoTypes.Create))
                {
                    var createDtoDeclaration = new DtoModel(DtoTypes.Create);
                    createDtoDeclaration.Name = attributeData.ConstructorArguments.Skip(2).FirstOrDefault().Value as string ?? $"{symbol.Name}{nameof(DtoTypes.Create)}Dto";
                    createDtoDeclaration.Namespace = dtoNamespace;
                    createDtoDeclaration.GenerateAdditionalStuff = attributeData.ConstructorArguments.Skip(3).FirstOrDefault().Value as GenerateAdditionalStuff? ?? GenerateAdditionalStuff.None;
                    yield return createDtoDeclaration;
                }
                if (dtoTypes.HasFlag(DtoTypes.Update))
                {
                    var updateDtoDeclaration = new DtoModel(DtoTypes.Update);
                    updateDtoDeclaration.Name = attributeData.ConstructorArguments.Skip(2).FirstOrDefault().Value as string ?? $"{symbol.Name}{nameof(DtoTypes.Update)}Dto";
                    updateDtoDeclaration.Namespace = dtoNamespace;
                    updateDtoDeclaration.GenerateAdditionalStuff = attributeData.ConstructorArguments.Skip(3).FirstOrDefault().Value as GenerateAdditionalStuff? ?? GenerateAdditionalStuff.None;
                    yield return updateDtoDeclaration;
                }
                if (dtoTypes.HasFlag(DtoTypes.View))
                {
                    var viewDtoDeclaration = new DtoModel(DtoTypes.View);
                    viewDtoDeclaration.Name = attributeData.ConstructorArguments.Skip(2).FirstOrDefault().Value as string ?? $"{symbol.Name}{nameof(DtoTypes.View)}Dto";
                    viewDtoDeclaration.Namespace = dtoNamespace;
                    viewDtoDeclaration.GenerateAdditionalStuff = attributeData.ConstructorArguments.Skip(3).FirstOrDefault().Value as GenerateAdditionalStuff? ?? GenerateAdditionalStuff.None;
                    yield return viewDtoDeclaration;
                }
            }

            foreach (var dtoModel in dtoModels)
            {
                var dtoType = dtoModel.Key;
                var propertiesDeclarations = symbol.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(p => p.GetAttributes().Any(a => a.AttributeClass.Name == Constants.DtoPropertyAttributeMetadataName &&
                                                            (a.ConstructorArguments.FirstOrDefault().Value as DtoTypes? ?? DtoTypes.None).HasFlag(dtoType)))
                    .Select(property =>
                    {
                        var propertyType = property.Type.ToDisplayString();
                        var propertyTypeSymbol = semanticModel.Compilation.GetTypeByMetadataName(propertyType);
                        var propertyAttributes = property.GetAttributes();
                        var dtoPropertyAttribute = propertyAttributes.FirstOrDefault(
                            a => a.AttributeClass.Name == Constants.DtoPropertyAttributeMetadataName &&
                                (a.ConstructorArguments.FirstOrDefault().Value as DtoTypes? ?? DtoTypes.None).HasFlag(dtoType));
                        var ignoreDtoPropertyAttribute = propertyAttributes.FirstOrDefault(
                            a => a.AttributeClass.Name == Constants.IgnoreDtoPropertyAttributeMetadataName &&
                                (a.ConstructorArguments.FirstOrDefault().Value as DtoTypes? ?? DtoTypes.None).HasFlag(dtoType));

                        return new PropertyModel(propertyTypeSymbol,
                                                dtoPropertyAttribute?.ConstructorArguments.Skip(1).FirstOrDefault().Value as string ?? property.Name,
                                                dtoPropertyAttribute?.ConstructorArguments.Skip(2).FirstOrDefault().Value as bool? ?? false);
                    });
            }

            foreach (var dtoModel in dtoModels)
            {
                var source = Constants.DtoOutputTypeDeclarationTemplate.Render(dtoModel);
                context.AddSource($"{dtoModel.Value.Name}.cs", source);
            }
        }
    }
}

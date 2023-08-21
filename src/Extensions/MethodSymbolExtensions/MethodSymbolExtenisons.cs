/*
 * MethodSymbolExtenisons.cs
 *
 *   Created: 2023-08-20-03:34:05
 *   Modified: 2023-08-20-03:34:05
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.CodeGeneration.MethodSymbolExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

public static class MethodSymbolInvocationExtensions
{
    public static object InvokeMethod(this IMethodSymbol methodSymbol, Compilation compilation, object instance, params object[] arguments)
    {
        var invocationExpression = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ThisExpression(),
                SyntaxFactory.IdentifierName(methodSymbol.Name)
            ),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(arguments.Select(arg =>
                    SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                        arg is string ? SyntaxKind.StringLiteralExpression :
                        SyntaxKind.NumericLiteralExpression,
                        arg is int i ? SyntaxFactory.Literal(i) :
                        arg is string s ? SyntaxFactory.Literal(s) :
                        arg is long l ? SyntaxFactory.Literal(l) :
                        SyntaxFactory.Literal(null as string)
                    ))
                ))
            )
        );

        var syntaxTree = CSharpSyntaxTree.ParseText(invocationExpression.ToString());
        var compilationUnit = syntaxTree.GetCompilationUnitRoot();
        var expressionStatement = compilationUnit.DescendantNodes().OfType<ExpressionStatementSyntax>().FirstOrDefault();

        if (expressionStatement != null)
        {
            var expression = expressionStatement.Expression;
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var expressionSymbol = semanticModel.GetSymbolInfo(expression).Symbol;

            if (expressionSymbol is IMethodSymbol methodInvocationSymbol)
            {
                var methodInfo = Type.GetType(methodInvocationSymbol.ContainingType.ToDisplayString())
                    .GetMethod(methodInvocationSymbol.Name);

                var methodInvocation = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), instance, methodInfo);
                return methodInvocation();
            }
        }

        return null;
    }
}

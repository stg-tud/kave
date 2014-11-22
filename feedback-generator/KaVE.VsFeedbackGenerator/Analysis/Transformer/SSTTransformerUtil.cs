/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Dennis Albrecht
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Model.Names;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Expressions;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Analysis.Util;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public static class SSTTransformerUtil
    {
        public static Expression AsExpression(this IList<string> references)
        {
            if (references.Any())
            {
                return ComposedExpression.Create(references.ToArray());
            }
            return new ConstantExpression();
        }

        public static Expression GetScopedReferences(this ICSharpTreeNode node, ITransformerContext context)
        {
            var scope = context.Factory.Scope();
            var refCollectorContext = new ReferenceCollectorContext(context, scope);
            node.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            if (scope.Body.Any())
            {
                // TODO: use BlockExpression here
                return refCollectorContext.References.AsExpression();
            }
            return refCollectorContext.References.AsExpression();
        }

        public static Expression GetReferences(this ICSharpTreeNode node, ITransformerContext context)
        {
            var refCollectorContext = new ReferenceCollectorContext(context);
            node.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            return refCollectorContext.References.AsExpression();
        }

        public static string[] GetArguments(this ICSharpTreeNode node, ITransformerContext context)
        {
            var argCollectorContext = new ArgumentCollectorContext(context);
            node.Accept(context.Factory.ArgumentCollector(), argCollectorContext);
            return argCollectorContext.Arguments.ToArray();
        }

        public static string GetArgument(this ICSharpTreeNode node, ITransformerContext context)
        {
            var argCollectorContext = new ArgumentCollectorContext(context);
            node.Accept(context.Factory.ArgumentCollector(), argCollectorContext);
            Asserts.That(argCollectorContext.Arguments.Count == 1);
            return argCollectorContext.Arguments[0];
        }

        public static string GetReference(this ICSharpTreeNode node, ITransformerContext context)
        {
            var refCollectorContext = new ReferenceCollectorContext(context);
            node.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            Asserts.That(refCollectorContext.References.Count == 1);
            return refCollectorContext.References[0];
        }

        public static IScope GetScope(this ICSharpTreeNode node, ITransformerContext context)
        {
            var scopeContext = new ScopeTransformerContext(context, context.Factory.Scope());
            node.Accept(context.Factory.ScopeTransformer(), scopeContext);
            return scopeContext.Scope;
        }

        public static void ProcessAssignment(this ICSharpTreeNode node, ITransformerContext context, string dest)
        {
            node.Accept(context.Factory.AssignmentGenerator(), new AssignmentGeneratorContext(context, dest));
        }

        public static InvocationExpression CreateInvocation(this string callee, IMethodName method, string[] args)
        {
            if (callee == null)
            {
                return new InvocationExpression(method, args);
            }
            return new InvocationExpression(callee, method, args);
        }
    }
}
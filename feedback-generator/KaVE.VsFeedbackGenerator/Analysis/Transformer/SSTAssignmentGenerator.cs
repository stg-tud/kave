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

using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using KaVE.VsFeedbackGenerator.Analysis.Util;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class AssignmentGeneratorContext : ITransformerContext
    {
        public AssignmentGeneratorContext(ITransformerContext context, string dest)
            : this(context.Factory, context.Generator, context.Scope, dest) {}

        private AssignmentGeneratorContext(ISSTFactory factory,
            ITempVariableGenerator generator,
            IScope scope,
            string dest)
        {
            Generator = generator;
            Factory = factory;
            Scope = scope;
            Dest = dest;
        }

        public ISSTFactory Factory { get; private set; }
        public ITempVariableGenerator Generator { get; private set; }
        public IScope Scope { get; private set; }
        public readonly string Dest;
    }

    public class SSTAssignmentGenerator : BaseSSTTransformer<AssignmentGeneratorContext>
    {
        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression cSharpLiteralExpressionParam,
            AssignmentGeneratorContext context)
        {
            context.Scope.Body.Add(new Assignment(context.Dest, new ConstantExpression()));
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam,
            AssignmentGeneratorContext context)
        {
            var name = referenceExpressionParam.NameIdentifier.Name;
            context.Scope.Body.Add(new Assignment(context.Dest, ComposedExpression.Create(name)));
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam,
            AssignmentGeneratorContext context)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                context,
                (callee, method, args, retType) =>
                    context.Scope.Body.Add(new Assignment(context.Dest, new InvocationExpression(callee, method, args))));
        }

        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam,
            AssignmentGeneratorContext context)
        {
            AddAssignmentAfterCollectingReferences(binaryExpressionParam, context);
        }

        public override void VisitArrayCreationExpression(IArrayCreationExpression arrayCreationExpressionParam,
            AssignmentGeneratorContext context)
        {
            AddAssignmentAfterCollectingReferences(arrayCreationExpressionParam.ArrayInitializer, context);
        }

        private static void AddAssignmentAfterCollectingReferences(ICSharpTreeNode treeNode,
            AssignmentGeneratorContext context)
        {
            var refCollectorContext = new ReferenceCollectorContext(context);
            treeNode.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            var references = refCollectorContext.References.ToArray();
            if (references.Any())
            {
                context.Scope.Body.Add(new Assignment(context.Dest, ComposedExpression.Create(references)));
            }
            else
            {
                context.Scope.Body.Add(new Assignment(context.Dest, new ConstantExpression()));
            }
        }
    }
}
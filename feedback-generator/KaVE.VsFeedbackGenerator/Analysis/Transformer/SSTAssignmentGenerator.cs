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

using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class SSTAssignmentGenerator : BaseSSTTransformer<AssignmentGeneratorContext>
    {
        public override void VisitArrayCreationExpression(IArrayCreationExpression arrayCreationExpressionParam,
            AssignmentGeneratorContext context)
        {
            context.Scope.Body.Add(
                new Assignment(context.Dest, arrayCreationExpressionParam.ArrayInitializer.GetReferences(context)));
        }

        public override void VisitAsExpression(IAsExpression asExpressionParam, AssignmentGeneratorContext context)
        {
            context.Scope.Body.Add(new Assignment(context.Dest, asExpressionParam.Operand.GetReferences(context)));
        }

        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam,
            AssignmentGeneratorContext context)
        {
            context.Scope.Body.Add(new Assignment(context.Dest, binaryExpressionParam.GetReferences(context)));
        }

        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression cSharpLiteralExpressionParam,
            AssignmentGeneratorContext context)
        {
            context.Scope.Body.Add(new Assignment(context.Dest, new ConstantExpression()));
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam,
            AssignmentGeneratorContext context)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                context,
                (callee, method, args, retType) =>
                    context.Scope.Body.Add(new Assignment(context.Dest, callee.CreateInvocation(method, args))));
        }

        public override void VisitIsExpression(IIsExpression isExpressionParam, AssignmentGeneratorContext context)
        {
            context.Scope.Body.Add(new Assignment(context.Dest, isExpressionParam.Operand.GetReferences(context)));
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam,
            AssignmentGeneratorContext context)
        {
            var name = referenceExpressionParam.NameIdentifier.Name;
            context.Scope.Body.Add(new Assignment(context.Dest, ComposedExpression.Create(name)));
        }

        public override void VisitThisExpression(IThisExpression thisExpressionParam, AssignmentGeneratorContext context)
        {
            context.Scope.Body.Add(new Assignment(context.Dest, ComposedExpression.Create("this")));
        }
    }
}
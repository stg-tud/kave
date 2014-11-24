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
using JetBrains.Util;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using KaVE.VsFeedbackGenerator.Analysis.Transformer.Context;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class SSTReferenceCollector : BaseSSTTransformer<ReferenceCollectorContext>
    {
        public override void VisitArrayCreationExpression(IArrayCreationExpression arrayCreationExpressionParam,
            ReferenceCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, arrayCreationExpressionParam.Type().GetName()));
            context.Scope.Body.Add(
                new Assignment(tmp, arrayCreationExpressionParam.ArrayInitializer.GetReferences(context)));
            context.References.Add(tmp);
        }

        public override void VisitArrayInitializer(IArrayInitializer arrayInitializerParam,
            ReferenceCollectorContext context)
        {
            arrayInitializerParam.ElementInitializers.ForEach(i => i.Accept(this, context));
        }

        public override void VisitAsExpression(IAsExpression asExpressionParam, ReferenceCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, asExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, asExpressionParam.Operand.GetReferences(context)));
            context.References.Add(tmp);
        }

        public override void VisitAssignmentExpression(IAssignmentExpression assignmentExpressionParam,
            ReferenceCollectorContext context)
        {
            var dest = assignmentExpressionParam.Dest.GetReference(context);
            assignmentExpressionParam.Source.ProcessAssignment(context, dest);
            context.References.Add(dest);
        }

        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam,
            ReferenceCollectorContext context)
        {
            binaryExpressionParam.LeftOperand.Accept(this, context);
            binaryExpressionParam.RightOperand.Accept(this, context);
        }

        public override void VisitCastExpression(ICastExpression castExpressionParam, ReferenceCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, castExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, castExpressionParam.Op.GetReferences(context)));
            context.References.Add(tmp);
        }

        public override void VisitConditionalTernaryExpression(
            IConditionalTernaryExpression conditionalTernaryExpressionParam,
            ReferenceCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, conditionalTernaryExpressionParam.Type().GetName()));
            context.Scope.Body.Add(
                new Assignment(
                    tmp,
                    new IfElseExpression
                    {
                        Condition = conditionalTernaryExpressionParam.ConditionOperand.GetScopedReferences(context),
                        ThenExpression = conditionalTernaryExpressionParam.ThenResult.GetScopedReferences(context),
                        ElseExpression = conditionalTernaryExpressionParam.ElseResult.GetScopedReferences(context)
                    }));
            context.References.Add(tmp);
        }

        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression cSharpLiteralExpressionParam,
            ReferenceCollectorContext context) {}

        public override void VisitDefaultExpression(IDefaultExpression defaultExpressionParam,
            ReferenceCollectorContext context) {}

        public override void VisitExpressionInitializer(IExpressionInitializer expressionInitializerParam,
            ReferenceCollectorContext context)
        {
            expressionInitializerParam.Value.Accept(this, context);
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam,
            ReferenceCollectorContext context)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                context,
                (callee, method, args, retType) =>
                {
                    var tmp = context.Generator.GetNextVariableName();
                    context.Scope.Body.Add(new VariableDeclaration(tmp, retType));
                    context.Scope.Body.Add(new Assignment(tmp, callee.CreateInvocation(method, args)));
                    context.References.Add(tmp);
                });
        }

        public override void VisitIsExpression(IIsExpression isExpressionParam, ReferenceCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, isExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, isExpressionParam.Operand.GetReferences(context)));
            context.References.Add(tmp);
        }

        public override void VisitParenthesizedExpression(IParenthesizedExpression parenthesizedExpressionParam,
            ReferenceCollectorContext context)
        {
            parenthesizedExpressionParam.Expression.Accept(this, context);
        }

        public override void VisitPostfixOperatorExpression(IPostfixOperatorExpression postfixOperatorExpressionParam,
            ReferenceCollectorContext context)
        {
            var reference = postfixOperatorExpressionParam.Operand.GetReference(context);
            context.Scope.Body.Add(new Assignment(reference, ComposedExpression.Create(reference)));
            context.References.Add(reference);
        }

        public override void VisitPrefixOperatorExpression(IPrefixOperatorExpression prefixOperatorExpressionParam,
            ReferenceCollectorContext context)
        {
            var reference = prefixOperatorExpressionParam.Operand.GetReference(context);
            context.Scope.Body.Add(new Assignment(reference, ComposedExpression.Create(reference)));
            context.References.Add(reference);
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam,
            ReferenceCollectorContext context)
        {
            context.References.Add(referenceExpressionParam.NameIdentifier.Name);
        }

        public override void VisitThisExpression(IThisExpression thisExpressionParam, ReferenceCollectorContext context)
        {
            context.References.Add("this");
        }

        public override void VisitTypeofExpression(ITypeofExpression typeofExpressionParam,
            ReferenceCollectorContext context) {}

        public override void VisitUnaryOperatorExpression(IUnaryOperatorExpression unaryOperatorExpressionParam,
            ReferenceCollectorContext context)
        {
            unaryOperatorExpressionParam.Operand.Accept(this, context);
        }
    }
}
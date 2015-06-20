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
 */

using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using KaVE.ReSharper.Commons.Analysis.Transformer.Context;

namespace KaVE.ReSharper.Commons.Analysis.Transformer
{
    public class SSTReferenceCollector : BaseSSTTransformer<ReferenceCollectorContext>
    {
        public override void VisitArrayCreationExpression(IArrayCreationExpression arrayCreationExpressionParam,
            ReferenceCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(SSTUtil.Declare(tmp, arrayCreationExpressionParam.Type().GetName()));
            context.Scope.Body.Add(
                SSTUtil.AssignmentToLocal(tmp, arrayCreationExpressionParam.ArrayInitializer.GetReferences(context)));
            context.References.Add(tmp);
        }

        public override void VisitArrayInitializer(IArrayInitializer arrayInitializerParam,
            ReferenceCollectorContext context)
        {
            arrayInitializerParam.ElementInitializers.ForEach(i => i.Accept(this, context));
        }

        public override void VisitAsExpression(IAsExpression asExpressionParam, ReferenceCollectorContext context)
        {
            asExpressionParam.Operand.Accept(this, context);
        }

        public override void VisitAssignmentExpression(IAssignmentExpression assignmentExpressionParam,
            ReferenceCollectorContext context)
        {
            var dest = assignmentExpressionParam.Dest.GetReference(context);
            var source = assignmentExpressionParam.Source;
            if (source != null)
            {
                source.ProcessAssignment(context, dest);
                context.References.Add(dest);
            }
        }

        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam,
            ReferenceCollectorContext context)
        {
            binaryExpressionParam.LeftOperand.Accept(this, context);
            binaryExpressionParam.RightOperand.Accept(this, context);
        }

        public override void VisitCastExpression(ICastExpression castExpressionParam, ReferenceCollectorContext context)
        {
            castExpressionParam.Op.Accept(this, context);
        }

        public override void VisitConditionalTernaryExpression(
            IConditionalTernaryExpression conditionalTernaryExpressionParam,
            ReferenceCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(SSTUtil.Declare(tmp, conditionalTernaryExpressionParam.Type().GetName()));
            context.Scope.Body.Add(
                SSTUtil.AssignmentToLocal(
                    tmp,
                    null));
            //new IfElseExpression
            //       {
            //          Condition = conditionalTernaryExpressionParam.ConditionOperand.GetScopedReferences(context),
            //         ThenExpression = conditionalTernaryExpressionParam.ThenResult.GetScopedReferences(context),
            //        ElseExpression = conditionalTernaryExpressionParam.ElseResult.GetScopedReferences(context)
            //   }));
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
            try
            {
                HandleInvocationExpression(
                    invocationExpressionParam,
                    context,
                    (callee, method, args, retType) =>
                    {
                        var tmp = context.Generator.GetNextVariableName();
                        context.Scope.Body.Add(SSTUtil.Declare(tmp, retType));
                        context.Scope.Body.Add(SSTUtil.AssignmentToLocal(tmp, callee.CreateInvocation(method, args)));
                        context.References.Add(tmp);
                    });
            }
            catch (AssertException)
            {
                // suppress KaVE errors
            }
        }

        public override void VisitIsExpression(IIsExpression isExpressionParam, ReferenceCollectorContext context)
        {
            isExpressionParam.Operand.Accept(this, context);
        }

        public override void VisitParenthesizedExpression(IParenthesizedExpression parenthesizedExpressionParam,
            ReferenceCollectorContext context)
        {
            parenthesizedExpressionParam.Expression.Accept(this, context);
        }

        public override void VisitPostfixOperatorExpression(IPostfixOperatorExpression postfixOperatorExpressionParam,
            ReferenceCollectorContext context)
        {
            postfixOperatorExpressionParam.Operand.Accept(this, context);
        }

        public override void VisitPrefixOperatorExpression(IPrefixOperatorExpression prefixOperatorExpressionParam,
            ReferenceCollectorContext context)
        {
            prefixOperatorExpressionParam.Operand.Accept(this, context);
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam,
            ReferenceCollectorContext context)
        {
            if (referenceExpressionParam.NameIdentifier != null)
            {
                context.References.Add(referenceExpressionParam.NameIdentifier.Name);
            }
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
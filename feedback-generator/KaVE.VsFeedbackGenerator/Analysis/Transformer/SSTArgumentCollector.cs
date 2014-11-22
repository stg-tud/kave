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
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class SSTArgumentCollector : BaseSSTTransformer<ArgumentCollectorContext>
    {
        public override void VisitArgumentList(IArgumentList argumentListParam, ArgumentCollectorContext context)
        {
            argumentListParam.Arguments.ForEach(argument => argument.Accept(this, context));
        }

        public override void VisitArrayCreationExpression(IArrayCreationExpression arrayCreationExpressionParam,
            ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, arrayCreationExpressionParam.Type().GetName()));
            context.Scope.Body.Add(
                new Assignment(tmp, arrayCreationExpressionParam.ArrayInitializer.GetReferences(context)));
            context.Arguments.Add(tmp);
        }

        public override void VisitAsExpression(IAsExpression asExpressionParam, ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, asExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, asExpressionParam.Operand.GetReferences(context)));
            context.Arguments.Add(tmp);
        }

        public override void VisitAssignmentExpression(IAssignmentExpression assignmentExpressionParam,
            ArgumentCollectorContext context)
        {
            var dest = assignmentExpressionParam.Dest.GetReference(context);
            assignmentExpressionParam.Source.Accept(
                context.Factory.AssignmentGenerator(),
                new AssignmentGeneratorContext(context, dest));
            context.Arguments.Add(dest);
        }

        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam,
            ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, binaryExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, binaryExpressionParam.GetReferences(context)));
            context.Arguments.Add(tmp);
        }

        public override void VisitCastExpression(ICastExpression castExpressionParam, ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, castExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, castExpressionParam.Op.GetReferences(context)));
            context.Arguments.Add(tmp);
        }

        public override void VisitCSharpArgument(ICSharpArgument cSharpArgumentParam, ArgumentCollectorContext context)
        {
            cSharpArgumentParam.Value.Accept(this, context);
        }

        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression cSharpLiteralExpressionParam,
            ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, cSharpLiteralExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, new ConstantExpression()));
            context.Arguments.Add(tmp);
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam,
            ArgumentCollectorContext context)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                context,
                (callee, method, args, retType) =>
                {
                    var tmp = context.Generator.GetNextVariableName();
                    context.Scope.Body.Add(new VariableDeclaration(tmp, retType));
                    context.Scope.Body.Add(new Assignment(tmp, callee.CreateInvocation(method, args)));
                    context.Arguments.Add(tmp);
                });
        }

        public override void VisitIsExpression(IIsExpression isExpressionParam, ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, isExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, isExpressionParam.Operand.GetReferences(context)));
            context.Arguments.Add(tmp);
        }

        public override void VisitParenthesizedExpression(IParenthesizedExpression parenthesizedExpressionParam,
            ArgumentCollectorContext context)
        {
            parenthesizedExpressionParam.Expression.Accept(this, context);
        }

        public override void VisitPostfixOperatorExpression(IPostfixOperatorExpression postfixOperatorExpressionParam,
            ArgumentCollectorContext context)
        {
            var reference = postfixOperatorExpressionParam.Operand.GetReference(context);
            context.Scope.Body.Add(new Assignment(reference, ComposedExpression.Create(reference)));
            context.Arguments.Add(reference);
        }

        public override void VisitPrefixOperatorExpression(IPrefixOperatorExpression prefixOperatorExpressionParam,
            ArgumentCollectorContext context)
        {
            var reference = prefixOperatorExpressionParam.Operand.GetReference(context);
            context.Scope.Body.Add(new Assignment(reference, ComposedExpression.Create(reference)));
            context.Arguments.Add(reference);
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam,
            ArgumentCollectorContext context)
        {
            context.Arguments.Add(referenceExpressionParam.NameIdentifier.Name);
        }

        public override void VisitThisExpression(IThisExpression thisExpressionParam, ArgumentCollectorContext context)
        {
            context.Arguments.Add("this");
        }

        public override void VisitTypeofExpression(ITypeofExpression typeofExpressionParam,
            ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, typeofExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, new ConstantExpression()));
            context.Arguments.Add(tmp);
        }

        public override void VisitUnaryOperatorExpression(IUnaryOperatorExpression unaryOperatorExpressionParam,
            ArgumentCollectorContext context)
        {
            var tmp = context.Generator.GetNextVariableName();
            context.Scope.Body.Add(new VariableDeclaration(tmp, unaryOperatorExpressionParam.Type().GetName()));
            context.Scope.Body.Add(new Assignment(tmp, unaryOperatorExpressionParam.Operand.GetReferences(context)));
            context.Arguments.Add(tmp);
        }
    }
}
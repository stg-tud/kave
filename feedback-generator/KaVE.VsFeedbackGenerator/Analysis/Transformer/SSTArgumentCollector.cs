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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ArgumentCollectorContext : ITransformerContext
    {
        public ArgumentCollectorContext(ITransformerContext context)
            : this(context.Factory, context.Generator, context.Scope) {}

        private ArgumentCollectorContext(ISSTFactory factory,
            ITempVariableGenerator generator,
            IScope scope)
        {
            Factory = factory;
            Generator = generator;
            Scope = scope;
            Arguments = new List<string>();
        }

        public ISSTFactory Factory { get; private set; }
        public ITempVariableGenerator Generator { get; private set; }
        public IScope Scope { get; private set; }
        public readonly IList<string> Arguments;
    }

    public class SSTArgumentCollector : BaseSSTTransformer<ArgumentCollectorContext>
    {
        public override void VisitArgumentList(IArgumentList argumentListParam, ArgumentCollectorContext context)
        {
            argumentListParam.Arguments.ForEach(argument => argument.Accept(this, context));
        }

        public override void VisitCSharpArgument(ICSharpArgument cSharpArgumentParam, ArgumentCollectorContext context)
        {
            cSharpArgumentParam.Value.Accept(this, context);
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam,
            ArgumentCollectorContext context)
        {
            context.Arguments.Add(referenceExpressionParam.NameIdentifier.Name);
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
                    context.Scope.Body.Add(new Assignment(tmp, new InvocationExpression(callee, method, args)));
                    context.Arguments.Add(tmp);
                });
        }

        public override void VisitThisExpression(IThisExpression thisExpressionParam, ArgumentCollectorContext context)
        {
            context.Arguments.Add("this");
        }
    }
}
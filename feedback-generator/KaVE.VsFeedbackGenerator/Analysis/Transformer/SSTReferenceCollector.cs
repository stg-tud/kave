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

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ReferenceCollectorContext : ITransformerContext
    {
        public ReferenceCollectorContext(ITransformerContext context)
            : this(context.Factory, context.Generator, context.Scope) {}

        private ReferenceCollectorContext(ISSTFactory factory,
            ITempVariableGenerator generator,
            IScope scope)
        {
            Factory = factory;
            Generator = generator;
            Scope = scope;
            References = new List<string>();
        }

        public ISSTFactory Factory { get; private set; }
        public ITempVariableGenerator Generator { get; private set; }
        public IScope Scope { get; private set; }
        public readonly IList<string> References;
    }

    public class SSTReferenceCollector : BaseSSTTransformer<ReferenceCollectorContext>
    {
        public override void VisitBinaryExpression(IBinaryExpression binaryExpressionParam,
            ReferenceCollectorContext context)
        {
            binaryExpressionParam.LeftOperand.Accept(this, context);
            binaryExpressionParam.RightOperand.Accept(this, context);
        }

        public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam,
            ReferenceCollectorContext context)
        {
            context.References.Add(referenceExpressionParam.NameIdentifier.Name);
        }

        public override void VisitArrayInitializer(IArrayInitializer arrayInitializerParam,
            ReferenceCollectorContext context)
        {
            arrayInitializerParam.ElementInitializers.ForEach(i => i.Accept(this, context));
        }

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
                    context.Scope.Body.Add(new Assignment(tmp, new InvocationExpression(callee, method, args)));
                    context.References.Add(tmp);
                });
        }

        public override void VisitThisExpression(IThisExpression thisExpressionParam, ReferenceCollectorContext context)
        {
            context.References.Add("this");
        }
    }
}
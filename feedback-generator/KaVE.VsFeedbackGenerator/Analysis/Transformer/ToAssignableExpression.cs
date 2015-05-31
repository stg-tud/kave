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
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;
using KaVE.VsFeedbackGenerator.Analysis.Util;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ToAssignableExpression : TreeNodeVisitor<IList<IStatement>, IAssignableExpression>
    {
        private readonly CompletionTargetMarker _marker;

        public ToAssignableExpression(UniqueVariableNameGenerator nameGen, CompletionTargetMarker marker)
        {
            _marker = marker;
        }

        public override IAssignableExpression VisitExpressionInitializer(IExpressionInitializer exprInit,
            IList<IStatement> context)
        {
            return exprInit.Value.Accept(this, context);
        }

        public override IAssignableExpression VisitCSharpLiteralExpression(ICSharpLiteralExpression litExpr,
            IList<IStatement> context)
        {
            var isNull = litExpr.ConstantValue.IsPureNull(CSharpLanguage.Instance);
            if (isNull)
            {
                return new NullExpression();
            }
            return new ConstantValueExpression();
        }

        public override IAssignableExpression VisitReferenceExpression(IReferenceExpression expr,
            IList<IStatement> context)
        {
            var hasName = expr.NameIdentifier != null;
            var hasQualifier = expr.QualifierExpression != null;

            if (hasName)
            {
                var refName = expr.NameIdentifier.Name;
                if (expr == _marker.AffectedNode)
                {
                    return new CompletionExpression
                    {
                        Token = refName
                    };
                }
                return new ReferenceExpression {Reference = new VariableReference {Identifier = refName}};
            }
            return new UnknownExpression();
        }
    }
}
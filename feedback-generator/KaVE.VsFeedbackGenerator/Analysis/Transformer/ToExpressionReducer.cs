/*
 * Copyright 2014 Technische Universitšt Darmstadt
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
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Expressions.Basic;
using KaVE.VsFeedbackGenerator.Analysis.Util;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ToBasicExpressionReducer : TreeNodeVisitor<IList<Statement>, Expression>
    {
        public ToBasicExpressionReducer(UniqueVariableNameGenerator nameGen) {}

        public override Expression VisitExpressionInitializer(IExpressionInitializer exprInit, IList<Statement> context)
        {
            return exprInit.Value.Accept(this, context);
        }

        public override Expression VisitCSharpLiteralExpression(ICSharpLiteralExpression litExpr,
            IList<Statement> context)
        {
            var isNull = litExpr.ConstantValue.IsPureNull(CSharpLanguage.Instance);
            if (isNull)
            {
                return new NullExpression();
            }
            return new ConstantValueExpression();
        }

        public override Expression VisitReferenceExpression(IReferenceExpression refExpr, IList<Statement> context)
        {
            var id = refExpr.NameIdentifier.Name;
            return new ReferenceExpression {Identifier = id};
        }
    }
}
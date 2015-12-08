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

using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;

// ReSharper disable once CheckNamespace

namespace KaVE.RS.Commons.Analysis.Transformer
{
    public partial class ExpressionVisitor
    {
        public override IAssignableExpression VisitUnaryOperatorExpression(IUnaryOperatorExpression expr,
            IList<IStatement> context)
        {
            var lit = expr.Operand as ICSharpLiteralExpression;
            if (lit != null)
            {
                switch (expr.UnaryOperatorType)
                {
                    case UnaryOperatorType.MINUS:
                        return ToConst(lit, false);
                    case UnaryOperatorType.PLUS:
                        return ToConst(lit, true);
                }
            }
            switch (expr.UnaryOperatorType)
            {
                case UnaryOperatorType.EXCL:
                    return new UnaryExpression
                    {
                        Operator = UnaryOperator.Not,
                        Operand = ToSimpleExpression(expr.Operand, context)
                    };
            }
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitParenthesizedExpression(IParenthesizedExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }
    }
}
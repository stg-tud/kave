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
using JetBrains.ReSharper.Psi.CSharp.Parsing;
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
        #region logical

        public override IAssignableExpression VisitConditionalAndExpression(IConditionalAndExpression expr,
            IList<IStatement> context)
        {
            return new BinaryExpression
            {
                LeftOperand = ToSimpleExpression(expr.LeftOperand, context),
                Operator = BinaryOperator.And,
                RightOperand = ToSimpleExpression(expr.RightOperand, context)
            };
        }

        public override IAssignableExpression VisitConditionalOrExpression(IConditionalOrExpression expr,
            IList<IStatement> context)
        {
            return new BinaryExpression
            {
                LeftOperand = ToSimpleExpression(expr.LeftOperand, context),
                Operator = BinaryOperator.Or,
                RightOperand = ToSimpleExpression(expr.RightOperand, context)
            };
        }

        public override IAssignableExpression VisitEqualityExpression(IEqualityExpression expr,
            IList<IStatement> context)
        {
            return new BinaryExpression
            {
                LeftOperand = ToSimpleExpression(expr.LeftOperand, context),
                Operator =
                    expr.EqualityType == EqualityExpressionType.EQEQ ? BinaryOperator.Equal : BinaryOperator.NotEqual,
                RightOperand = ToSimpleExpression(expr.RightOperand, context)
            };
        }

        public override IAssignableExpression VisitRelationalExpression(IRelationalExpression expr,
            IList<IStatement> context)
        {
            BinaryOperator op = BinaryOperator.Unknown;
            if (expr.OperatorSign.NodeType == CSharpTokenType.GT)
            {
                op = BinaryOperator.GreaterThan;
            }
            else if (expr.OperatorSign.NodeType == CSharpTokenType.GE)
            {
                op = BinaryOperator.GreaterThanOrEqual;
            }
            else if (expr.OperatorSign.NodeType == CSharpTokenType.LE)
            {
                op = BinaryOperator.LessThanOrEqual;
            }
            else if (expr.OperatorSign.NodeType == CSharpTokenType.LT)
            {
                op = BinaryOperator.LessThan;
            }

            return new BinaryExpression
            {
                LeftOperand = ToSimpleExpression(expr.LeftOperand, context),
                Operator = op,
                RightOperand = ToSimpleExpression(expr.RightOperand, context)
            };
        }

        #endregion

        #region arithmetic

        public override IAssignableExpression VisitAdditiveExpression(IAdditiveExpression expr,
            IList<IStatement> context)
        {
            BinaryOperator op = BinaryOperator.Unknown;
            if (expr.OperatorSign.NodeType == CSharpTokenType.PLUS)
            {
                op = BinaryOperator.Plus;
            }
            else if (expr.OperatorSign.NodeType == CSharpTokenType.MINUS)
            {
                op = BinaryOperator.Minus;
            }

            return new BinaryExpression
            {
                LeftOperand = ToSimpleExpression(expr.LeftOperand, context),
                Operator = op,
                RightOperand = ToSimpleExpression(expr.RightOperand, context)
            };
        }

        public override IAssignableExpression VisitMultiplicativeExpression(IMultiplicativeExpression expr,
            IList<IStatement> context)
        {
            BinaryOperator op = BinaryOperator.Unknown;
            if (expr.OperatorSign.NodeType == CSharpTokenType.PERC)
            {
                op = BinaryOperator.Modulo;
            }
            else if (expr.OperatorSign.NodeType == CSharpTokenType.DIV)
            {
                op = BinaryOperator.Divide;
            }
            else if (expr.OperatorSign.NodeType == CSharpTokenType.ASTERISK)
            {
                op = BinaryOperator.Multiply;
            }

            return new BinaryExpression
            {
                LeftOperand = ToSimpleExpression(expr.LeftOperand, context),
                Operator = op,
                RightOperand = ToSimpleExpression(expr.RightOperand, context)
            };
        }

        #endregion

        #region bitwise

        public override IAssignableExpression VisitBitwiseAndExpression(IBitwiseAndExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitBitwiseExclusiveOrExpression(IBitwiseExclusiveOrExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitBitwiseInclusiveOrExpression(IBitwiseInclusiveOrExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitShiftExpression(IShiftExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        #endregion
    }
}
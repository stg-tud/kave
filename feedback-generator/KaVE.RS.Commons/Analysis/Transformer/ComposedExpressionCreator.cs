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
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils.Collections;
using IStatement = KaVE.Commons.Model.SSTs.IStatement;

namespace KaVE.RS.Commons.Analysis.Transformer
{
    public class ComposedExpressionCreator : TreeNodeVisitor<IList<IStatement>>
    {
        public static IAssignableExpression Create(ExpressionVisitor expressionVisitor,
            ICSharpExpression expr,
            IList<IStatement> context)
        {
            var visitor = new ComposedExpressionCreator(expressionVisitor);
            expr.Accept(visitor, context);
            return new ComposedExpression {References = visitor._collectedRefs};
        }

        private readonly ExpressionVisitor _expressionVisitor;
        private readonly IKaVEList<IVariableReference> _collectedRefs = Lists.NewList<IVariableReference>();

        public ComposedExpressionCreator(ExpressionVisitor expressionVisitor)
        {
            _expressionVisitor = expressionVisitor;
        }

        public override void VisitCSharpLiteralExpression(ICSharpLiteralExpression expr,
            IList<IStatement> context)
        {
            // Do nothing!
        }

        public override void VisitAdditiveExpression(IAdditiveExpression expr, IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitMultiplicativeExpression(IMultiplicativeExpression expr, IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitParenthesizedExpression(IParenthesizedExpression expr, IList<IStatement> context)
        {
            expr.Expression.Accept(this, context);
        }

        public override void VisitCastExpression(ICastExpression expr, IList<IStatement> context)
        {
            expr.Op.Accept(this, context);
        }

        public override void VisitConditionalTernaryExpression(IConditionalTernaryExpression expr,
            IList<IStatement> context)
        {
            expr.ConditionOperand.Accept(this, context);
            expr.ThenResult.Accept(this, context);
            expr.ElseResult.Accept(this, context);
        }

        public override void VisitConditionalAndExpression(IConditionalAndExpression expr,
            IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitConditionalOrExpression(IConditionalOrExpression expr, IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitEqualityExpression(IEqualityExpression expr, IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitBitwiseAndExpression(IBitwiseAndExpression expr, IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitBitwiseExclusiveOrExpression(IBitwiseExclusiveOrExpression expr,
            IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitBitwiseInclusiveOrExpression(IBitwiseInclusiveOrExpression expr,
            IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitUnaryOperatorExpression(IUnaryOperatorExpression expr, IList<IStatement> context)
        {
            expr.Operand.Accept(this, context);
        }

        public override void VisitShiftExpression(IShiftExpression expr, IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitRelationalExpression(IRelationalExpression expr, IList<IStatement> context)
        {
            foreach (var csExpr in expr.OperatorOperands)
            {
                csExpr.Accept(this, context);
            }
        }

        public override void VisitNode(ITreeNode node, IList<IStatement> context)
        {
            var csExpr = node as ICSharpExpression;
            if (csExpr != null)
            {
                var varRef = _expressionVisitor.ToVariableRef(csExpr, context);
                _collectedRefs.Add(varRef);
            }
        }
    }
}
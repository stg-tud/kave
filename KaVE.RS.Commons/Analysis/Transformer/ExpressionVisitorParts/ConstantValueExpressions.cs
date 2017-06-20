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

using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;

// ReSharper disable once CheckNamespace

namespace KaVE.RS.Commons.Analysis.Transformer
{
    public partial class ExpressionVisitor
    {
        public override IAssignableExpression VisitCSharpLiteralExpression(ICSharpLiteralExpression litExpr,
            IList<IStatement> context)
        {
            return ToConst(litExpr, true);
        }

        public override IAssignableExpression VisitDefaultExpression(IDefaultExpression expr, IList<IStatement> body)
        {
            return new ConstantValueExpression {Value = ConstantValueExpression.Default};
        }

        public override IAssignableExpression VisitUnsafeCodeSizeOfExpression(IUnsafeCodeSizeOfExpression expr,
            IList<IStatement> context)
        {
            return new ConstantValueExpression {Value = ConstantValueExpression.Sizeof};
        }

        public override IAssignableExpression VisitTypeofExpression(ITypeofExpression expr, IList<IStatement> body)
        {
            return new ConstantValueExpression {Value = ConstantValueExpression.Typeof};
        }

        private static IAssignableExpression ToConst(ICSharpLiteralExpression lit, bool isPositive)
        {
            var isNull = lit.ConstantValue.IsPureNull(CSharpLanguage.Instance);
            if (isNull)
            {
                return new ConstantValueExpression {Value = ConstantValueExpression.Null};
            }

            var val = lit.ConstantValue.Value;

            if (val is string)
            {
                return new ConstantValueExpression();
            }

            if (val is int)
            {
                var i = (int) val;
                if (!isPositive)
                {
                    i = -1*i;
                }
                var v = i == -1 || i == 0 || i == 1 || i == 2 ? i.ToString() : null;
                return new ConstantValueExpression {Value = v};
            }

            if (val is double)
            {
                var d = (double) val;
                if (!isPositive)
                {
                    d = -1*d;
                }
                string v = null;
                Func<double, double, bool> isEq = (a, b) => Math.Abs(a - b) < 0.000001;
                if (isEq(d, 0) || isEq(d, -1) || isEq(d, 1))
                {
                    v = string.Format("{0:0.0}", d).Replace(',', '.');
                }
                return new ConstantValueExpression {Value = v};
            }

            if (val is bool)
            {
                var b = (bool) val;
                return new ConstantValueExpression
                {
                    Value = b ? ConstantValueExpression.True : ConstantValueExpression.False
                };
            }

            return new ConstantValueExpression();
        }
    }
}
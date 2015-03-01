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

using KaVE.Model.SSTs.Expressions.Simple;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.Visitor;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Simple
{
    public class ConstantValueExpressionTest
    {
        [Test]
        public void Equality()
        {
            var a = new ConstantValueExpression();
            var b = new ConstantValueExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(0, a.GetHashCode());
            Assert.AreNotEqual(1, a.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ConstantValueExpression();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 7);

            Assert.AreEqual(sut, visitor.Expr);
            Assert.AreEqual(7, visitor.Context);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IConstantValueExpression Expr { get; private set; }
            public int Context { get; private set; }

            public override void Visit(IConstantValueExpression expr, int context)
            {
                Expr = expr;
                Context = context;
            }
        }
    }
}
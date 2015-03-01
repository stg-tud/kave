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

using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Statements
{
    public class ReturnStatementTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ReturnStatement();
            Assert.IsNull(sut.Expression);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ReturnStatement {Expression = new ConstantValueExpression()};
            Assert.AreEqual(new ConstantValueExpression(), sut.Expression);
        }

        [Test]
        public void Equality_default()
        {
            var a = new ReturnStatement();
            var b = new ReturnStatement();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_reallyTheSame()
        {
            var a = new ReturnStatement {Expression = new ConstantValueExpression()};
            var b = new ReturnStatement {Expression = new ConstantValueExpression()};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentIdentifier()
        {
            var a = new ReturnStatement {Expression = new ConstantValueExpression()};
            var b = new ReturnStatement {Expression = new NullExpression()};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ReturnStatement();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 1);

            Assert.AreEqual(sut, visitor.Statement);
            Assert.AreEqual(1, visitor.Context);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IReturnStatement Statement { get; private set; }
            public int Context { get; private set; }

            public override void Visit(IReturnStatement stmt, int context)
            {
                Statement = stmt;
                Context = context;
            }
        }
    }
}
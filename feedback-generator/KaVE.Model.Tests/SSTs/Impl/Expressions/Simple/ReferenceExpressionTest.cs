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

using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Expressions.Simple;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Visitor;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Simple
{
    internal class ReferenceExpressionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ReferenceExpression();
            Assert.Null(sut.Reference);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ReferenceExpression {Reference = Ref("a")};
            Assert.AreEqual(Ref("a"), sut.Reference);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ReferenceExpression();
            var b = new ReferenceExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new ReferenceExpression {Reference = Ref("a")};
            var b = new ReferenceExpression {Reference = Ref("a")};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIDentifier()
        {
            var a = new ReferenceExpression {Reference = Ref("a")};
            var b = new ReferenceExpression {Reference = Ref("b")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ReferenceExpression();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 9);

            Assert.AreEqual(sut, visitor.Expr);
            Assert.AreEqual(9, visitor.Context);
        }

        private static IReference Ref(string id)
        {
            return new VariableReference {Identifier = id};
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IReferenceExpression Expr { get; private set; }
            public int Context { get; private set; }

            public override void Visit(IReferenceExpression expr, int context)
            {
                Expr = expr;
                Context = context;
            }
        }
    }
}
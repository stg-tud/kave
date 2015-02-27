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
using System.Linq;
using KaVE.Model.Collections;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Model.SSTs.References;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Assignable
{
    public class ComposedExpressionTest
    {
        private TestVisitor _visitor;

        [SetUp]
        public void Setup()
        {
            _visitor = new TestVisitor();
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new ComposedExpression();
            Assert.IsNotNull(sut.References);
            Assert.AreEqual(0, sut.References.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ComposedExpression {References = Refs("a")};
            Assert.AreEqual(Refs("a"), sut.References);
        }

        [Test]
        public void SettingValues_StaticHelper()
        {
            var sut = ComposedExpression.New("a", "b");
            var expected = new[] {"a", "b"};
            Assert.AreEqual(expected, sut.References);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ComposedExpression();
            var b = new ComposedExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = ComposedExpression.New("b");
            var b = ComposedExpression.New("b");
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVariables()
        {
            var a = ComposedExpression.New("a");
            var b = ComposedExpression.New("b");
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ComposedExpression();
            Assert.Null(_visitor.Expr);
            sut.Accept(_visitor, 0);
            Assert.AreEqual(sut, _visitor.Expr);
        }

        private IList<IVariableReference> Refs(params string[] strRefs)
        {
            var refs = strRefs.ToList().Select(r => new VariableReference {Identifier = r});
            return Lists.NewListFrom<IVariableReference>(refs);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IComposedExpression Expr { get; private set; }

            public override void Visit(IComposedExpression expr, int context)
            {
                Expr = expr;
            }
        }
    }
}
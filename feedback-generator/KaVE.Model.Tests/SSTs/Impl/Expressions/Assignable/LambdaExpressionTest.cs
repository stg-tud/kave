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

using KaVE.Model.Collections;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Assignable
{
    public class LambdaExpressionTest
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
            var sut = new LambdaExpression();
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.Body);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new LambdaExpression();
            sut.Body.Add(new GotoStatement());

            var expected = Lists.NewList<IStatement>();
            expected.Add(new GotoStatement());

            Assert.AreEqual(expected, sut.Body);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new LambdaExpression();
            var b = new LambdaExpression();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new LambdaExpression();
            a.Body.Add(new GotoStatement());
            var b = new LambdaExpression();
            b.Body.Add(new GotoStatement());

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new LambdaExpression();
            a.Body.Add(new GotoStatement());
            var b = new LambdaExpression();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ComposedExpression();
            sut.Accept(_visitor, 13);
            Assert.AreEqual(sut, _visitor.Expr);
            Assert.AreEqual(13, _visitor.Context);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IExpressionCompletion Expr { get; private set; }
            public int Context { get; private set; }

            public override void Visit(IExpressionCompletion expr, int context)
            {
                Expr = expr;
                Context = context;
            }
        }

    }
}
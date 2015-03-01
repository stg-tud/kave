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
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Assignable
{
    public class LambdaExpressionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new LambdaExpression();
            Assert.NotNull(sut.Parameters);
            Assert.AreEqual(0, sut.Parameters.Count);
            Assert.NotNull(sut.Body);
            Assert.AreEqual(0, sut.Body.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new LambdaExpression();
            sut.Parameters.Add(new VariableDeclaration());
            sut.Body.Add(new GotoStatement());

            var expectedBody = Lists.NewList<IStatement>();
            expectedBody.Add(new GotoStatement());
            Assert.AreEqual(expectedBody, sut.Body);

            var expectedParameters = Lists.NewList<IVariableDeclaration>();
            expectedParameters.Add(new VariableDeclaration());
            Assert.AreEqual(expectedParameters, sut.Parameters);
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
            a.Parameters.Add(new VariableDeclaration());
            a.Body.Add(new GotoStatement());

            var b = new LambdaExpression();
            b.Parameters.Add(new VariableDeclaration());
            b.Body.Add(new GotoStatement());

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParameters()
        {
            var a = new LambdaExpression();
            a.Parameters.Add(new VariableDeclaration());
            var b = new LambdaExpression();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
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
            var sut = new LambdaExpression();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 5);
            Assert.AreEqual(sut, visitor.Expr);
            Assert.AreEqual(5, visitor.Context);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public ILambdaExpression Expr { get; private set; }
            public int Context { get; private set; }

            public override void Visit(ILambdaExpression expr, int context)
            {
                Expr = expr;
                Context = context;
            }
        }
    }
}
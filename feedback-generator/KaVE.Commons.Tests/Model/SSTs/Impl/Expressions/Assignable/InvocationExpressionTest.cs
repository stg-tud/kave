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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Assignable
{
    internal class InvocationExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new InvocationExpression();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Names.UnknownMethod, sut.MethodName);
            Assert.AreEqual(Lists.NewList<ISimpleExpression>(), sut.Parameters);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new InvocationExpression
            {
                Reference = SomeVarRef("a"),
                MethodName = GetMethod("b"),
                Parameters = {new NullExpression()}
            };
            Assert.AreEqual(SomeVarRef("a"), sut.Reference);
            Assert.AreEqual(GetMethod("b"), sut.MethodName);
            Assert.AreEqual(Lists.NewList(new NullExpression()), sut.Parameters);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new InvocationExpression();
            var b = new InvocationExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            Assert.AreEqual(GetMethod("a"), GetMethod("a"));

            var a = SSTUtil.InvocationExpression("o", GetMethod("A"), RefExprs("a", "b", "c"));
            var b = SSTUtil.InvocationExpression("o", GetMethod("A"), RefExprs("a", "b", "c"));
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new InvocationExpression {Reference = new VariableReference {Identifier = "a"}};
            var b = new InvocationExpression {Reference = new VariableReference {Identifier = "b"}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethod()
        {
            var a = new InvocationExpression {MethodName = GetMethod("A")};
            var b = new InvocationExpression {MethodName = GetMethod("B")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParameters()
        {
            var a = new InvocationExpression {Parameters = RefExprs("a")};
            var b = new InvocationExpression {Parameters = RefExprs("b")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new InvocationExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new InvocationExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new InvocationExpression());
        }
    }
}
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

using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Assignable
{
    internal class IndexAccessExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new IndexAccessExpression();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Lists.NewList<ISimpleExpression>(), sut.Indices);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new IndexAccessExpression
            {
                Reference = SomeVarRef(),
                Indices = {new ConstantValueExpression()}
            };

            Assert.AreEqual(SomeVarRef(), sut.Reference);
            Assert.AreEqual(new KaVEList<ISimpleExpression> { new ConstantValueExpression() }, sut.Indices);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new IndexAccessExpression();
            var b = new IndexAccessExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new IndexAccessExpression
            {
                Reference = SomeVarRef(),
                Indices = {new ConstantValueExpression()}
            };

            var b = new IndexAccessExpression
            {
                Reference = SomeVarRef(),
                Indices = {new ConstantValueExpression()}
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIndex()
        {
            var a = new IndexAccessExpression
            {
                Reference = SomeVarRef(),
                Indices = {new ConstantValueExpression {Value = "1"}}
            };

            var b = new IndexAccessExpression
            {
                Reference = SomeVarRef(),
                Indices = {new ConstantValueExpression {Value = "2"}}
            };

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVarRef()
        {
            var a = new IndexAccessExpression
            {
                Reference = SomeVarRef("i"),
                Indices = {new ConstantValueExpression()}
            };

            var b = new IndexAccessExpression
            {
                Reference = SomeVarRef("j"),
                Indices = {new ConstantValueExpression()}
            };

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new IndexAccessExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new IndexAccessExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new IndexAccessExpression());
        }
    }
}
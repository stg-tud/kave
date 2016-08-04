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
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Assignable
{
    internal class TypeCheckExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new TypeCheckExpression();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Names.UnknownType, sut.Type);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TypeCheckExpression
            {
                Type = Names.Type("p:int"),
                Reference = SomeVarRef()
            };

            Assert.AreEqual(Names.Type("p:int"), sut.Type);
            Assert.AreEqual(SomeVarRef(), sut.Reference);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TypeCheckExpression();
            var b = new TypeCheckExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TypeCheckExpression
            {
                Type = Names.Type("p:int"),
                Reference = SomeVarRef()
            };

            var b = new TypeCheckExpression
            {
                Type = Names.Type("p:int"),
                Reference = SomeVarRef()
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new TypeCheckExpression
            {
                Type = Names.Type("p:int"),
                Reference = SomeVarRef()
            };

            var b = new TypeCheckExpression
            {
                Type = Names.Type("p:string"),
                Reference = SomeVarRef()
            };

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVarRef()
        {
            var a = new TypeCheckExpression
            {
                Type = Names.Type("p:int"),
                Reference = SomeVarRef("i")
            };

            var b = new TypeCheckExpression
            {
                Type = Names.Type("p:int"),
                Reference = SomeVarRef("j")
            };

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new TypeCheckExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new TypeCheckExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new TypeCheckExpression());
        }
    }
}
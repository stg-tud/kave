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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Assignable
{
    internal class CastExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CastExpression();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(TypeName.UnknownName, sut.TargetType);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new CastExpression
            {
                TargetType = TypeName.Get("System.Int32, mscorlib, 4.0.0.0"),
                Reference = SomeVarRef()
            };

            Assert.AreEqual(TypeName.Get("System.Int32, mscorlib, 4.0.0.0"), sut.TargetType);
            Assert.AreEqual(SomeVarRef(), sut.Reference);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new CastExpression();
            var b = new CastExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new CastExpression
            {
                TargetType = TypeName.Get("System.Int32, mscorlib, 4.0.0.0"),
                Reference = SomeVarRef()
            };

            var b = new CastExpression
            {
                TargetType = TypeName.Get("System.Int32, mscorlib, 4.0.0.0"),
                Reference = SomeVarRef()
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTargetType()
        {
            var a = new CastExpression
            {
                TargetType = TypeName.Get("System.Int32, mscorlib, 4.0.0.0"),
                Reference = SomeVarRef()
            };

            var b = new CastExpression
            {
                TargetType = TypeName.Get("System.String, mscorlib, 4.0.0.0"),
                Reference = SomeVarRef()
            };

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVarRef()
        {
            var a = new CastExpression
            {
                TargetType = TypeName.Get("System.Int32, mscorlib, 4.0.0.0"),
                Reference = SomeVarRef("i")
            };

            var b = new CastExpression
            {
                TargetType = TypeName.Get("System.Int32, mscorlib, 4.0.0.0"),
                Reference = SomeVarRef("j")
            };

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new CastExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new CastExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new CastExpression());
        }
    }
}
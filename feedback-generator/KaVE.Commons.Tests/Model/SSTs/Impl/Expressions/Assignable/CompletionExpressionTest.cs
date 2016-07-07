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
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Expressions.Assignable
{
    internal class CompletionExpressionTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CompletionExpression();
            Assert.AreEqual("", sut.Token);
            Assert.Null(sut.VariableReference);
            Assert.Null(sut.TypeReference);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new CompletionExpression
            {
                VariableReference = SomeVarRef("i"),
                TypeReference = Names.UnknownType,
                Token = "t"
            };
            Assert.AreEqual(SomeVarRef("i"), sut.VariableReference);
            Assert.AreEqual(Names.UnknownType, sut.TypeReference);
            Assert.AreEqual("t", sut.Token);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new CompletionExpression();
            var b = new CompletionExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new CompletionExpression
            {
                VariableReference = SomeVarRef("i"),
                Token = "t",
                TypeReference = Names.UnknownType
            };
            var b = new CompletionExpression
            {
                VariableReference = SomeVarRef("i"),
                Token = "t",
                TypeReference = Names.UnknownType
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentObjectReference()
        {
            var a = new CompletionExpression {VariableReference = SomeVarRef("i")};
            var b = new CompletionExpression {VariableReference = SomeVarRef("j")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }


        [Test]
        public void Equality_DifferentToken()
        {
            var a = new CompletionExpression {Token = "t"};
            var b = new CompletionExpression {Token = "u"};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTypeReference()
        {
            var a = new CompletionExpression {TypeReference = Names.UnknownType};
            var b = new CompletionExpression {TypeReference = Names.Type("System.Int32, mscore, 4.0.0.0")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new CompletionExpression();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new CompletionExpression();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new CompletionExpression());
        }
    }
}
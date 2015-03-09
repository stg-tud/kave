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

using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.References;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Assignable
{
    public class CompletionExpressionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CompletionExpression();
            Assert.Null(sut.Token);
            Assert.Null(sut.ObjectReference);
            Assert.Null(sut.TypeReference);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new CompletionExpression
            {
                ObjectReference = Ref("i"),
                TypeReference = TypeName.UnknownName,
                Token = "t"
            };
            Assert.AreEqual(Ref("i"), sut.ObjectReference);
            Assert.AreEqual(TypeName.UnknownName, sut.TypeReference);
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
                ObjectReference = Ref("i"),
                Token = "t",
                TypeReference = TypeName.UnknownName
            };
            var b = new CompletionExpression
            {
                ObjectReference = Ref("i"),
                Token = "t",
                TypeReference = TypeName.UnknownName
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentObjectReference()
        {
            var a = new CompletionExpression { ObjectReference = Ref("i") };
            var b = new CompletionExpression { ObjectReference = Ref("j") };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }


        [Test]
        public void Equality_DifferentToken()
        {
            var a = new CompletionExpression { Token = "t" };
            var b = new CompletionExpression { Token = "u" };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTypeReference()
        {
            var a = new CompletionExpression { TypeReference = TypeName.UnknownName };
            var b = new CompletionExpression { TypeReference = TypeName.Get("System.Int32, mscore, 4.0.0.0") };
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

        private static IVariableReference Ref(string id)
        {
            return new VariableReference {Identifier = id};
        }
    }
}
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
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Model.SSTs.References;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs
{
    public class CompletionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new Completion();
            Assert.Null(sut.Token);
            Assert.Null(sut.ObjectReference);
            Assert.Null(sut.TypeReference);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new Completion
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
            var a = new Completion();
            var b = new Completion();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new Completion
            {
                ObjectReference = Ref("i"),
                Token = "t",
                TypeReference = TypeName.UnknownName
            };
            var b = new Completion
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
            var a = new Completion {ObjectReference = Ref("i")};
            var b = new Completion {ObjectReference = Ref("j")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }


        [Test]
        public void Equality_DifferentToken()
        {
            var a = new Completion {Token = "t"};
            var b = new Completion {Token = "u"};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTypeReference()
        {
            var a = new Completion {TypeReference = TypeName.UnknownName};
            var b = new Completion {TypeReference = TypeName.Get("System.Int32, mscore, 4.0.0.0")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new Completion();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 2);

            Assert.AreEqual(sut, visitor.Expr);
            Assert.AreEqual(2, visitor.Context);
        }

        private static IVariableReference Ref(string id)
        {
            return new VariableReference {Identifier = id};
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public ICompletion Expr { get; private set; }
            public int Context { get; private set; }

            public override void Visit(ICompletion expr, int context)
            {
                Expr = expr;
                Context = context;
            }
        }
    }
}
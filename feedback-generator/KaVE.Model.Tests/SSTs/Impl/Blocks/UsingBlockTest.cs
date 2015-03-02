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
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Impl.Blocks;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Model.SSTs.References;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Blocks
{
    public class UsingBlockTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new UsingBlock();
            Assert.IsNull(sut.Reference);
            Assert.NotNull(sut.Body);
            Assert.AreEqual(0, sut.Body.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new UsingBlock {Reference = Ref("a")};
            sut.Body.Add(new ReturnStatement());
            Assert.AreEqual(Ref("a"), sut.Reference);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.Body);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new UsingBlock();
            var b = new UsingBlock();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new UsingBlock {Reference = Ref("b")};
            a.Body.Add(new ReturnStatement());
            var b = new UsingBlock {Reference = Ref("b")};
            b.Body.Add(new ReturnStatement());
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new UsingBlock {Reference = Ref("a")};
            var b = new UsingBlock {Reference = Ref("b")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new UsingBlock();
            a.Body.Add(new ContinueStatement());
            var b = new UsingBlock();
            b.Body.Add(new GotoStatement());
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new UsingBlock();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 11);

            Assert.AreEqual(sut, visitor.Statement);
            Assert.AreEqual(11, visitor.Context);
        }

        private static IVariableReference Ref(string id)
        {
            return new VariableReference {Identifier = id};
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public IUsingBlock Statement { get; private set; }
            public int Context { get; private set; }

            public override void Visit(IUsingBlock stmt, int context)
            {
                Statement = stmt;
                Context = context;
            }
        }
    }
}
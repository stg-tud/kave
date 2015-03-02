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
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Impl.Visitor;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Blocks
{
    internal class CatchBlockTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new CatchBlock();
            Assert.Null(sut.Exception);
            Assert.NotNull(sut.Body);
            Assert.AreEqual(0, sut.Body.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new CatchBlock {Exception = new VariableDeclaration()};
            sut.Body.Add(new ReturnStatement());

            Assert.AreEqual(new VariableDeclaration(), sut.Exception);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.Body);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new CatchBlock();
            var b = new CatchBlock();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new CatchBlock {Exception = new VariableDeclaration()};
            a.Body.Add(new ReturnStatement());
            var b = new CatchBlock {Exception = new VariableDeclaration()};
            b.Body.Add(new ReturnStatement());
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentException()
        {
            var a = new CatchBlock {Exception = new VariableDeclaration()};
            var b = new CatchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new CatchBlock();
            a.Body.Add(new ReturnStatement());
            var b = new CatchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new CatchBlock();
            var visitor = new TestVisitor();
            sut.Accept(visitor, 2);

            Assert.AreEqual(sut, visitor.Statement);
            Assert.AreEqual(2, visitor.Context);
        }

        internal class TestVisitor : AbstractNodeVisitor<int>
        {
            public ICatchBlock Statement { get; private set; }
            public int Context { get; private set; }

            public override void Visit(ICatchBlock stmt, int context)
            {
                Statement = stmt;
                Context = context;
            }
        }
    }
}
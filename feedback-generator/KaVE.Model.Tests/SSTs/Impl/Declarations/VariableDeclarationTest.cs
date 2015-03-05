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
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Declarations;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Declarations
{
    internal class VariableDeclarationTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new VariableDeclaration();
            Assert.Null(sut.Reference);
            Assert.Null(sut.Type);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new VariableDeclaration
            {
                Reference = SSTUtil.VariableReference("a"),
                Type = TypeName.UnknownName
            };

            Assert.AreEqual(SSTUtil.VariableReference("a"), sut.Reference);
            Assert.AreEqual(TypeName.UnknownName, sut.Type);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new VariableDeclaration();
            var b = new VariableDeclaration();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyEquals()
        {
            var a = new VariableDeclaration {Reference = SSTUtil.VariableReference("a"), Type = TypeName.UnknownName};
            var b = new VariableDeclaration {Reference = SSTUtil.VariableReference("a"), Type = TypeName.UnknownName};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new VariableDeclaration {Reference = SSTUtil.VariableReference("a")};
            var b = new VariableDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new VariableDeclaration {Type = TypeName.UnknownName};
            var b = new VariableDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new VariableDeclaration();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new VariableDeclaration();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }
    }
}
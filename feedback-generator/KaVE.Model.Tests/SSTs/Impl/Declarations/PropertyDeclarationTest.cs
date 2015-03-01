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
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Statements;
using KaVE.Model.SSTs.Visitor;
using Moq;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Declarations
{
    internal class PropertyDeclarationTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new PropertyDeclaration();
            Assert.Null(sut.Name);
            Assert.NotNull(sut.Get);
            Assert.AreEqual(0, sut.Get.Count);
            Assert.NotNull(sut.Set);
            Assert.AreEqual(0, sut.Set.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new PropertyDeclaration {Name = PropertyName.UnknownName};
            sut.Get.Add(new ReturnStatement());
            sut.Set.Add(new ContinueStatement());

            var expectedGet = Lists.NewList<IStatement>();
            expectedGet.Add(new ReturnStatement());

            var expectedSet = Lists.NewList<IStatement>();
            expectedSet.Add(new ContinueStatement());

            Assert.AreEqual(PropertyName.UnknownName, sut.Name);
            Assert.AreEqual(expectedGet, sut.Get);
            Assert.AreEqual(expectedSet, sut.Set);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new PropertyDeclaration();
            var b = new PropertyDeclaration();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyEquals()
        {
            var a = new PropertyDeclaration {Name = PropertyName.UnknownName};
            var b = new PropertyDeclaration {Name = PropertyName.UnknownName};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new PropertyDeclaration {Name = PropertyName.UnknownName};
            var b = new PropertyDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentGet()
        {
            var a = new PropertyDeclaration();
            var b = new PropertyDeclaration();

            a.Get.Add(new ReturnStatement());

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSet()
        {
            var a = new PropertyDeclaration();
            var b = new PropertyDeclaration();

            a.Set.Add(new ReturnStatement());

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorImplementation()
        {
            var sut = new PropertyDeclaration();
            var @visitor = new Mock<ISSTNodeVisitor<object>>();
            var context = new object();

            sut.Accept(@visitor.Object, context);

            @visitor.Verify(v => v.Visit(sut, context));
        }
    }
}
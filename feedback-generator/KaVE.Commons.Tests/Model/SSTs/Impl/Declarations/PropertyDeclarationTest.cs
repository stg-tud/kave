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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Declarations
{
    internal class PropertyDeclarationTest
    {
        private static IPropertyName SomeProperty
        {
            get { return Names.Property("get [T1,P1] [T2,P2].Property()"); }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new PropertyDeclaration();
            Assert.AreEqual(Names.UnknownProperty, sut.Name);
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.Get);
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.Set);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new PropertyDeclaration
            {
                Name = SomeProperty,
                Get = {new ReturnStatement()},
                Set = {new ContinueStatement()}
            };

            var expectedGet = Lists.NewList(new ReturnStatement());
            var expectedSet = Lists.NewList(new ContinueStatement());

            Assert.AreEqual(SomeProperty, sut.Name);
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
            var a = new PropertyDeclaration
            {
                Name = SomeProperty,
                Get = {new ReturnStatement()},
                Set = {new ContinueStatement()}
            };
            var b = new PropertyDeclaration
            {
                Name = SomeProperty,
                Get = {new ReturnStatement()},
                Set = {new ContinueStatement()}
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new PropertyDeclaration {Name = SomeProperty};
            var b = new PropertyDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentGet()
        {
            var a = new PropertyDeclaration {Get = {new ReturnStatement()}};
            var b = new PropertyDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSet()
        {
            var a = new PropertyDeclaration {Set = {new ReturnStatement()}};
            var b = new PropertyDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new PropertyDeclaration();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new PropertyDeclaration();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new PropertyDeclaration());
        }
    }
}
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
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Declarations
{
    internal class MethodDeclarationTest
    {
        private readonly IMethodName _mA = Names.Method("[T1,P1] [T2,P2].A()");
        private readonly IMethodName _mB = Names.Method("[T1,P1] [T2,P2].B()");

        [Test]
        public void DefaultValues()
        {
            var sut = new MethodDeclaration();
            Assert.AreEqual(Names.UnknownMethod, sut.Name);
            Assert.False(sut.IsEntryPoint);
            Assert.AreEqual(Lists.NewList<IMethodDeclaration>(), sut.Body);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new MethodDeclaration
            {
                Name = _mA,
                IsEntryPoint = true,
                Body = {new ReturnStatement()}
            };

            Assert.AreEqual(_mA, sut.Name);
            Assert.True(sut.IsEntryPoint);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.Body);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new MethodDeclaration();
            var b = new MethodDeclaration();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new MethodDeclaration
            {
                Name = _mA,
                IsEntryPoint = true,
                Body = {new ReturnStatement()}
            };
            var b = new MethodDeclaration
            {
                Name = _mA,
                IsEntryPoint = true,
                Body = {new ReturnStatement()}
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new MethodDeclaration
            {
                Name = _mA
            };
            var b = new MethodDeclaration
            {
                Name = _mB
            };

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentEntryPoint()
        {
            var a = new MethodDeclaration {IsEntryPoint = true};
            var b = new MethodDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new MethodDeclaration();
            a.Body.Add(new ContinueStatement());
            var b = new MethodDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new MethodDeclaration();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new MethodDeclaration();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new MethodDeclaration());
        }
    }
}
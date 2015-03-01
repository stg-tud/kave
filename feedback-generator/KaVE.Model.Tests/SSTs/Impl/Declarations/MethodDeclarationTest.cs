﻿/*
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
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Statements;
using KaVE.Model.SSTs.Visitor;
using Moq;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Declarations
{
    internal class MethodDeclarationTest
    {
        private readonly IMethodName _mA = MethodName.Get("MA");
        private readonly IMethodName _mB = MethodName.Get("MB");

        [Test]
        public void DefaultValues()
        {
            var sut = new MethodDeclaration();
            Assert.Null(sut.Name);
            Assert.False(sut.IsEntryPoint);
            Assert.NotNull(sut.Body);
            Assert.AreEqual(0, sut.Body.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new MethodDeclaration
            {
                Name = _mA,
                IsEntryPoint = true
            };
            sut.Body.Add(new ReturnStatement());

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
                IsEntryPoint = true
            };
            a.Body.Add(new ReturnStatement());
            var b = new MethodDeclaration
            {
                Name = _mA,
                IsEntryPoint = true
            };
            b.Body.Add(new ReturnStatement());

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
        public void VisitorImplementation()
        {
            var sut = new MethodDeclaration();
            var @visitor = new Mock<ISSTNodeVisitor<object>>();
            var context = new object();

            sut.Accept(@visitor.Object, context);

            @visitor.Verify(v => v.Visit(sut, context));
        }
    }
}
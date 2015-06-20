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

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.Names;
using KaVE.RS.Commons.Analysis;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis
{
    [TestFixture]
    public class MethodRefTest
    {
        private IMethodName _nA;
        private IMethodName _nB;
        private IMethodDeclaration _dA;
        private IMethodDeclaration _dB;
        private IMethod _mA;
        private IMethod _mB;

        [SetUp]
        public void Setup()
        {
            _nA = MockName();
            _nB = MockName();
            _dA = MockDeclaration();
            _dB = MockDeclaration();
            _mA = MockMethod();
            _mB = MockMethod();
        }

        [Test]
        public void ShouldSetProperties()
        {
            var actual = MethodRef.CreateLocalReference(_nA, _mA, _dA);

            Assert.AreSame(_nA, actual.Name);
            Assert.AreSame(_dA, actual.Declaration);
            Assert.AreSame(_mA, actual.Method);
            Assert.IsFalse(actual.IsAssemblyReference);
        }

        [Test]
        public void ShouldSetProperties2()
        {
            var actual = MethodRef.CreateAssemblyReference(_nA, _mA);

            Assert.AreSame(_nA, actual.Name);
            Assert.IsNull(actual.Declaration);
            Assert.AreSame(_mA, actual.Method);
            Assert.IsTrue(actual.IsAssemblyReference);
        }

        [Test]
        public void HashcodeAndEquals_Same()
        {
            var a = MethodRef.CreateLocalReference(_nA, _mA, _dA);
            var b = MethodRef.CreateLocalReference(_nA, _mA, _dA);
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashcodeAndEquals_OneIsDifferent()
        {
            var a = MethodRef.CreateLocalReference(_nA, _mA, _dA);
            var b = MethodRef.CreateLocalReference(_nB, _mB, _dA);
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashcodeAndEquals_OneIsDifferent2()
        {
            var a = MethodRef.CreateLocalReference(_nA, _mA, _dA);
            var b = MethodRef.CreateLocalReference(_nA, _mB, _dB);
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashcodeAndEquals_BothDifferent()
        {
            var a = MethodRef.CreateLocalReference(_nA, _mA, _dA);
            var b = MethodRef.CreateLocalReference(_nB, _mB, _dB);
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashcodeAndEquals_DifferentMethod()
        {
            var a = MethodRef.CreateLocalReference(_nA, _mA, _dA);
            var b = MethodRef.CreateLocalReference(_nA, _mB, _dA);
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void HashcodeAndEquals_AssemblyReferenceIsDifferent()
        {
            var a = MethodRef.CreateLocalReference(_nA, _mA, _dA);
            var b = MethodRef.CreateAssemblyReference(_nA, _mA);
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }


        private static IMethodDeclaration MockDeclaration()
        {
            return new Mock<IMethodDeclaration>().Object;
        }

        private static IMethodName MockName()
        {
            return new Mock<IMethodName>().Object;
        }

        private IMethod MockMethod()
        {
            return new Mock<IMethod>().Object;
        }
    }
}
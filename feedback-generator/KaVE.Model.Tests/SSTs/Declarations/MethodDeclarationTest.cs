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

using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Declarations
{
    internal class MethodDeclarationTest
    {
        private readonly IMethodName _mA = MethodName.Get("MA");
        private readonly IMethodName _mB = MethodName.Get("MB");

        [Test]
        public void HashCodeAndEquals_DefaultInstantitation()
        {
            var a = new MethodDeclaration();
            var b = new MethodDeclaration();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_SameName()
        {
            var a = new MethodDeclaration
            {
                Name = _mA
            };
            var b = new MethodDeclaration
            {
                Name = _mA
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_DifferentName()
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
        public void HashCodeAndEquals_SameBody()
        {
            var a = new MethodDeclaration();
            a.Body.Add(new VariableDeclaration("i", null));
            var b = new MethodDeclaration();
            b.Body.Add(new VariableDeclaration("i", null));

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void HashCodeAndEquals_DifferentBody()
        {
            var a = new MethodDeclaration();
            a.Body.Add(new VariableDeclaration("i", null));
            var b = new MethodDeclaration();
            b.Body.Add(new ContinueStatement());

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
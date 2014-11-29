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
using KaVE.Model.SSTs.Expressions;
using KaVE.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Expressions
{
    public class InvocationExpressionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new InvocationExpression();
            Assert.IsNull(sut.Identifier);
            Assert.IsNull(sut.Name);
            Assert.IsNull(sut.Parameters);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var a = new InvocationExpression
            {
                Identifier = "a1",
                Name = GetMethod("A2"),
                Parameters = new[] {"a3"}
            };
            Assert.AreEqual("a1", a.Identifier);
            Assert.AreEqual(GetMethod("A2"), a.Name);
            Assert.AreEqual(new[] {"a3"}, a.Parameters);
        }

        [Test]
        public void CustomConstructor_NonStatic()
        {
            var a = new InvocationExpression("a1", GetMethod("B1"), "c1");
            Assert.AreEqual("a1", a.Identifier);
            Assert.AreEqual(GetMethod("B1"), a.Name);
            Assert.AreEqual(new[] {"c1"}, a.Parameters);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void CustomConstructor_NonStaticAssert()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new InvocationExpression("a1", GetStaticMethod("B1"), "c1");
        }

        [Test]
        public void CustomConstructor_Static()
        {
            var a = new InvocationExpression(GetStaticMethod("B2"), "c2");
            Assert.AreEqual("", a.Identifier);
            Assert.AreEqual(GetStaticMethod("B2"), a.Name);
            Assert.AreEqual(new[] {"c2"}, a.Parameters);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void CustomConstructor_StaticAssert()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new InvocationExpression(GetMethod("B2"), "c2");
        }

        [Test]
        public void Equality_Default()
        {
            var a = new InvocationExpression();
            var b = new InvocationExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            Assert.AreEqual(GetMethod("a"), GetMethod("a"));

            var a = new InvocationExpression("o", GetMethod("A"), "a", "b", "c");
            var b = new InvocationExpression("o", GetMethod("A"), "a", "b", "c");
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIdentifier()
        {
            var a = new InvocationExpression {Identifier = "a"};
            var b = new InvocationExpression {Identifier = "b"};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethod()
        {
            var a = new InvocationExpression {Name = GetMethod("A")};
            var b = new InvocationExpression {Name = GetMethod("B")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParameters()
        {
            var a = new InvocationExpression {Parameters = new[] {"a"}};
            var b = new InvocationExpression {Parameters = new[] {"b"}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        private static IMethodName GetMethod(string simpleName)
        {
            var methodName = string.Format(
                "[System.String, mscore, 4.0.0.0] [System.String, mscore, 4.0.0.0].{0}()",
                simpleName);
            return MethodName.Get(methodName);
        }

        private static IMethodName GetStaticMethod(string simpleName)
        {
            var methodName = string.Format(
                "static [System.String, mscore, 4.0.0.0] [System.String, mscore, 4.0.0.0].{0}()",
                simpleName);
            return MethodName.Get(methodName);
        }
    }
}
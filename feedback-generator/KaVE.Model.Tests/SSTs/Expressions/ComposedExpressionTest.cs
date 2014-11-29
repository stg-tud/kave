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

using KaVE.Model.SSTs.Expressions;
using KaVE.Utils;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Expressions
{
    public class ComposedExpressionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ComposedExpression();
            Assert.IsNull(sut.Variables);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ComposedExpression {Variables = new[] {"a"}};
            var expected = new[] {"a"};
            Assert.That(expected.DeepEquals(sut.Variables));
        }

        [Test]
        public void SettingValues_StaticHelper()
        {
            var sut = ComposedExpression.Create("a", "b");
            var expected = new[] {"a", "b"};
            Assert.AreEqual(expected, sut.Variables);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ComposedExpression();
            var b = new ComposedExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new ComposedExpression {Variables = new[] {"b"}};
            var b = new ComposedExpression {Variables = new[] {"b"}};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentVariables()
        {
            var a = new ComposedExpression {Variables = new[] {"a"}};
            var b = new ComposedExpression {Variables = new[] {"b"}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_EmptyArray()
        {
            var a = new ComposedExpression {Variables = null};
            var b = new ComposedExpression {Variables = new string[] {}};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
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
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Statements
{
    public class AssignmentTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new Assignment();
            Assert.Null(sut.Identifier);
            Assert.Null(sut.Value);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new Assignment
            {
                Identifier = "x",
                Value = new ConstantExpression()
            };
            Assert.AreEqual("x", sut.Identifier);
            Assert.AreEqual(new ConstantExpression(), sut.Value);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new Assignment();
            var b = new Assignment();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new Assignment("a", new ConstantExpression());
            var b = new Assignment("a", new ConstantExpression());
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new Assignment {Identifier = "a"};
            var b = new Assignment {Identifier = "b"};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentValue()
        {
            var a = new Assignment {Value = new ConstantExpression()};
            var b = new Assignment {Value = new ComposedExpression()};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
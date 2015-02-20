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
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Expressions
{
    public class MemberAccessExpressionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new MemberAccessExpression();
            Assert.AreEqual(null, sut.Identifier);
            Assert.AreEqual(null, sut.MemberName);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new MemberAccessExpression
            {
                Identifier = "i",
                MemberName = "m"
            };
            Assert.AreEqual("i", sut.Identifier);
            Assert.AreEqual("m", sut.MemberName);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new MemberAccessExpression();
            var b = new MemberAccessExpression();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new MemberAccessExpression
            {
                Identifier = "i",
                MemberName = "m"
            };
            var b = new MemberAccessExpression
            {
                Identifier = "i",
                MemberName = "m"
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIdentifier()
        {
            var a = new MemberAccessExpression {Identifier = "i"};
            var b = new MemberAccessExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMemberName()
        {
            var a = new MemberAccessExpression {MemberName = "m"};
            var b = new MemberAccessExpression();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
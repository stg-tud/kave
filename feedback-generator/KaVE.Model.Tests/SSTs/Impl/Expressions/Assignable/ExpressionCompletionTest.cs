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

using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Expressions.Assignable
{
    public class ExpressionCompletionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ExpressionCompletion();
            Assert.Null(sut.Token);
            Assert.Null(sut.Identifier);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ExpressionCompletion
            {
                Identifier = "i",
                Token = "t"
            };
            Assert.AreEqual("i", sut.Identifier);
            Assert.AreEqual("t", sut.Token);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ExpressionCompletion();
            var b = new ExpressionCompletion();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new ExpressionCompletion {Identifier = "i", Token = "t"};
            var b = new ExpressionCompletion {Identifier = "i", Token = "t"};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIdentifier()
        {
            var a = new ExpressionCompletion {Identifier = "i"};
            var b = new ExpressionCompletion {Identifier = "j"};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }


        [Test]
        public void Equality_DifferentToken()
        {
            var a = new ExpressionCompletion {Token = "t"};
            var b = new ExpressionCompletion {Token = "u"};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
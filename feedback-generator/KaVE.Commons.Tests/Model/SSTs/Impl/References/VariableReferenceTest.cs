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

using KaVE.Commons.Model.SSTs.Impl.References;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.References
{
    public class VariableReferenceTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new VariableReference();
            Assert.AreEqual("", sut.Identifier);
            Assert.True(sut.IsMissing);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new VariableReference {Identifier = "a"};
            Assert.False(sut.IsMissing);
            Assert.AreEqual("a", sut.Identifier);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new VariableReference();
            var b = new VariableReference();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new VariableReference {Identifier = "a"};
            var b = new VariableReference {Identifier = "a"};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new VariableReference {Identifier = "a"};
            var b = new VariableReference();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new VariableReference();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new VariableReference();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringIsImplemented()
        {
            var sut = new VariableReference {Identifier = "xyz"};
            var actual = sut.ToString();
            const string expected = "VariableReference(xyz)";
            Assert.AreEqual(expected, actual);
        }
    }
}
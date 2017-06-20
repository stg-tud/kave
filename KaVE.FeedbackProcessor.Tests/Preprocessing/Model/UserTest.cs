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

using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Model
{
    internal class UserTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new User();
            Assert.NotNull(sut.Identifiers);
            Assert.That(sut.Identifiers.Count == 0);
            Assert.NotNull(sut.Files);
            Assert.That(sut.Files.Count == 0);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new User
            {
                Files = {"a"},
                Identifiers = {"b"}
            };
            Assert.AreEqual(Sets.NewHashSet("a"), sut.Files);
            Assert.AreEqual(Sets.NewHashSet("b"), sut.Identifiers);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new User();
            var b = new User();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new User
            {
                Files = {"a"},
                Identifiers = {"b"}
            };
            var b = new User
            {
                Files = {"a"},
                Identifiers = {"b"}
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFiles()
        {
            var a = new User
            {
                Files = {"a"}
            };
            var b = new User();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }


        [Test]
        public void Equality_DifferentIds()
        {
            var a = new User
            {
                Identifiers = {"b"}
            };
            var b = new User();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new User());
        }
    }
}
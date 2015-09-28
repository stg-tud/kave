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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Collections
{
    internal class FifoCacheTest
    {
        private FifoCache<int, int> _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new FifoCache<int, int>(2);
        }

        [Test]
        public void CacheSizeIsSet()
        {
            Assert.AreEqual(1, new FifoCache<int, int>(1).CacheSize);
            Assert.AreEqual(2, new FifoCache<int, int>(2).CacheSize);
        }

        [Test]
        public void ValuesCanBeAdded()
        {
            _sut.SetValue(1, 11);
            AssertKeys(1);
            Assert.AreEqual(11, _sut.GetValue(1));
        }

        [Test]
        public void OldValuesAreKicked()
        {
            _sut.SetValue(1, 11);
            _sut.SetValue(2, 22);
            _sut.SetValue(3, 33);
            AssertKeys(2, 3);
        }

        [Test]
        public void KeysAreAccessible()
        {
            AssertKeys();
            _sut.SetValue(1, 11);
            AssertKeys(1);
            _sut.SetValue(2, 22);
            AssertKeys(1, 2);
            _sut.SetValue(3, 33);
            AssertKeys(2, 3);
        }

        [Test]
        public void ValuesAreNotRenewed()
        {
            _sut.SetValue(1, 11);
            _sut.SetValue(2, 22);
            _sut.SetValue(1, 11);
            _sut.SetValue(3, 33);
            AssertKeys(2, 3);
        }

        [Test]
        public void KeysCannotBeAddedTwice()
        {
            _sut.SetValue(1, 11);
            _sut.SetValue(1, 11);
            AssertKeys(1);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new FifoCache<int, int>(1);
            var b = new FifoCache<int, int>(1);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new FifoCache<int, int>(1);
            a.SetValue(1, 11);
            var b = new FifoCache<int, int>(1);
            b.SetValue(1, 11);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void Equality_DifferentSize()
        {
            var a = new FifoCache<int, int>(1);
            var b = new FifoCache<int, int>(2);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void Equality_DifferentValues()
        {
            var a = new FifoCache<int, int>(1);
            a.SetValue(1, 11);
            var b = new FifoCache<int, int>(1);
            b.SetValue(1, 12);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(_sut);
        }

        private void AssertKeys(params int[] expectedKeyArr)
        {
            var expecteds = Lists.NewListFrom(expectedKeyArr);
            Assert.AreEqual(expecteds, _sut.Keys);
        }
    }
}
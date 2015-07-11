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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Collections
{
    [TestFixture]
    internal class MultisetTest
    {
        [Test]
        public void Create()
        {
            var multiset = new Multiset<string> {{"foo", 2}, "bar"};

            Assert.AreEqual(3, multiset.Count());
            Assert.AreEqual(2, multiset.Count("foo"));
        }

        [Test]
        public void CreateFromIterable()
        {
            var multiset = new Multiset<string>(new[] {"foo", "bar", "foo"});

            Assert.AreEqual(3, multiset.Count());
            Assert.AreEqual(2, multiset.Count("foo"));
        }

        [Test]
        public void IsEnumerable()
        {
            var uut = new Multiset<string>(new[] {"foo", "bar", "bar"});

            Assert.AreEqual(3, Enumerable.Count(uut));
        }

        [Test]
        public void HasElementSet()
        {
            var uut = new Multiset<string>(new[] {"foo", "bar", "foo", "bar"});

            CollectionAssert.AreEqual(new[] {"foo", "bar"}, uut.ElementSet);
        }

        [Test]
        public void HasEntryMap()
        {
            var uut = new Multiset<string>(new[] {"foo", "bar", "foo", "bar"});

            var expected = new Dictionary<string, int> {{"foo", 2}, {"bar", 2}};
            CollectionAssert.AreEqual(expected, uut.EntryDictionary);
        }

        [Test]
        public void RemovesAll()
        {
            var uut = new Multiset<string> {{"a", 5}};

            uut.RemoveAll("a");

            Assert.AreEqual(0, uut.Count("a"));
        }
    }
}
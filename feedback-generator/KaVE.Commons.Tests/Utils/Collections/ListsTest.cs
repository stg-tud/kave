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
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Collections
{
    internal class ListsTest
    {
        [Test]
        public void NewListReturnsList()
        {
            var s = Lists.NewList<string>();
            Assert.IsTrue(s is List<string>);
        }

        [Test]
        public void NewListCanBeCreatedWithParams()
        {
            var s = Lists.NewList(1, 2);
            Assert.IsTrue(s.Count == 2);
            Assert.IsTrue(s.Contains(1));
            Assert.IsTrue(s.Contains(2));
        }

        [Test]
        public void ItemsCanBeAdded()
        {
            var a = Lists.NewList<string>();
            a.Add("a");
            var b = Lists.NewList("a");
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void MultipleItemsCanBeAdded()
        {
            var a = Lists.NewList<string>();
            a.AddAll(new []{"a","b"});
            var b = Lists.NewList("a", "b");
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void TwoEmptyLists()
        {
            var a = Lists.NewList<string>();
            var b = Lists.NewList<string>();
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TwoEmptyLists2()
        {
            var a = Lists.NewList<int>();
            var b = Lists.NewList<string>();
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(a.GetHashCode() != b.GetHashCode());
        }

        [Test]
        public void TwoNonEmptySets_Equal()
        {
            var a = Lists.NewList(1);
            var b = Lists.NewList(1);
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TwoNonEmptySets_OrderIsImportant()
        {
            var a = Lists.NewList(1, 2);
            var b = Lists.NewList(2, 1);
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TwoNonEmptySets_Diff()
        {
            var a = Lists.NewList<int>();
            a.Add(1);
            var b = Lists.NewList<int>();
            b.Add(2);
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TakeCareWhenAssertingEqualityWithNUnit()
        {
            var a = Lists.NewList(1, 2);
            var b = Lists.NewList(1, 2);
            CollectionAssert.AreEqual(a, b);
        }

        [Test]
        public void TakeCareWhenAssertingInequalityWithNUnit()
        {
            var a = Lists.NewList(1, 2);
            var b = Lists.NewList(2, 1);
            CollectionAssert.AreNotEqual(a, b);
        }

        [Test]
        public void CreatingSetFromEnumerable()
        {
            var input = new HashSet<int>(new[] {1, 2, 3});
            var a = Lists.NewListFrom(input);
            var b = Lists.NewList(1, 2, 3);
            Assert.AreEqual(a, b);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void NullsCannotBeAdded()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Lists.NewList<object>().Add(null);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void NullsCannotBeAddedWithStaticCreate()
        {
            Lists.NewList(new object[] {null});
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void NullsCannotBeAddedWithEnumerable()
        {
            Lists.NewListFrom(new object[] {null});
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new KaVEList<int>());
        }
    }
}
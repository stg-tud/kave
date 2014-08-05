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
 *    - 
 */

using System.Collections.Generic;
using KaVE.Model.Collections;
using NUnit.Framework;

namespace KaVE.Model.Tests.Collections
{
    [TestFixture]
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
    }
}
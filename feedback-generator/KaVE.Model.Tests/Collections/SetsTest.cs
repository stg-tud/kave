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

using System.Collections.Generic;
using KaVE.Model.Collections;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Model.Tests.Collections
{
    [TestFixture]
    internal class SetsTest
    {
        [Test]
        public void NewHashSetReturnsHashSet()
        {
            var s = Sets.NewHashSet<string>();
            Assert.IsTrue(s is HashSet<string>);
        }

        [Test]
        public void NewHashSetCanBeCreatedWithParams()
        {
            var s = Sets.NewHashSet(1, 2);
            Assert.IsTrue(s.Count == 2);
            Assert.IsTrue(s.Contains(1));
            Assert.IsTrue(s.Contains(2));
        }

        [Test]
        public void TwoEmptySets()
        {
            var a = Sets.NewHashSet<string>();
            var b = Sets.NewHashSet<string>();
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TwoEmptySets2()
        {
            var a = Sets.NewHashSet<int>();
            var b = Sets.NewHashSet<string>();
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(a.GetHashCode() != b.GetHashCode());
        }

        [Test]
        public void TwoNonEmptySets_Equal()
        {
            var a = Sets.NewHashSet(1);
            var b = Sets.NewHashSet(1);
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TwoNonEmptySets_Equal2()
        {
            var a = Sets.NewHashSet(1, 2);
            var b = Sets.NewHashSet(2, 1);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TwoNonEmptySets_Diff()
        {
            var a = Sets.NewHashSet<int>();
            a.Add(1);
            var b = Sets.NewHashSet<int>();
            b.Add(2);
            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void TakeCareWhenAssertingEqualityWithNUnit()
        {
            var a = Sets.NewHashSet(1, 2);
            var b = Sets.NewHashSet(2, 1);
            CollectionAssert.AreEquivalent(a, b);
        }

        [Test]
        public void CreatingSetFromEnumerable()
        {
            var input = new HashSet<int>(new[] {1, 2, 3});
            var a = Sets.NewHashSetFrom(input);
            var b = Sets.NewHashSet(1, 2, 3);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void SetContains()
        {
            var a = new BreakStatement();
            var b = new BreakStatement();
            Assert.AreNotSame(a, b);
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

            var sut = Sets.NewHashSet(a);
            Assert.True(sut.Contains(b));
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void NullsCannotBeAddedWithStaticCreate()
        {
            Sets.NewHashSet(new object[] {null});
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void NullsCannotBeAddedWithEnumerable()
        {
            Sets.NewHashSetFrom(new object[] {null});
        }
    }
}
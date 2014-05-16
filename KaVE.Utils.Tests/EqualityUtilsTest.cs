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
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace KaVE.Utils.Tests
{
    [TestFixture]
    class EqualityUtilsTest
    {
        [Test]
        public void ShouldDeclareSameInstanceEqual()
        {
            var obj = new TestClass(23);

            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(Equals(obj, obj));
        }

        [Test]
        public void ShouldDeclareEqualIstancesEquals()
        {
            var obj1 = new TestClass(42);
            var obj2 = new TestClass(42);

            Assert.IsTrue(obj1.Equals(obj2, other => other.Value == obj1.Value));
        }

        [Test]
        public void ShouldDeclareNullDifferentFromInstance()
        {
            var obj = new TestClass(0);

            Assert.IsFalse(obj.Equals(null, other => other.Value == obj.Value));
        }

        [Test]
        public void ShouldDeclareDifferentInstancesDifferent()
        {
            var obj1 = new TestClass(1);
            var obj2 = new TestClass(2);

            Assert.IsFalse(obj1.Equals(obj2, other => other.Value == obj1.Value));
        }

        [Test]
        public void ShouldDeclareObjectOfDifferentTypeDifferent()
        {
            var obj1 = new TestClass(23);
            var obj2 = new Object();

            Assert.IsFalse(obj1.Equals(obj2, other => other.Value == obj1.Value));
        }

        private class TestClass
        {
            public readonly int Value;

            public TestClass(int value)
            {
                Value = value;
            }
        }

        [Test]
        public void ShouldDeclareTwoDictionariesOfSetsEqualByContent()
        {
            var dict1 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar", "ups"}}
            };
            var dict2 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar", "ups"}}
            };

            Assert.IsTrue(dict1.DeepEquals(dict2));
        }

        [Test]
        public void ShouldDeclareTwoDictionariesOfSetsEqualByContentIndependentOfOrder()
        {
            var dict1 = new Dictionary<string, ISet<string>>
            {
                {"bli", new HashSet<string>{"bla", "blub"}},
                {"foo", new HashSet<string>{"bar", "ups"}}
            };
            var dict2 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar", "ups"}},
                {"bli", new HashSet<string>{"bla", "blub"}}
            };

            Assert.IsTrue(dict1.DeepEquals(dict2));
        }

        [Test]
        public void ShouldDeclareTwoDictionariesOfSetsUnequalIfSetsDiffer()
        {
            var dict1 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar", "ups"}}
            };
            var dict2 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar"}}
            };

            Assert.IsFalse(dict1.DeepEquals(dict2));
        }

        [Test]
        public void ShouldDeclareTwoDictionariesOfSetsUnequalIfMoreKeys()
        {
            var dict1 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar", "ups"}}
            };
            var dict2 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar", "ups"}},
                {"bli", new HashSet<string>{"bla", "blub"}}
            };

            Assert.IsFalse(dict1.DeepEquals(dict2));
        }

        [Test]
        public void ShouldDeclareTwoDictionariesOfSetsUnequalIfLessKeys()
        {
            var dict1 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar", "ups"}},
                {"bli", new HashSet<string>{"bla", "blub"}}
            };
            var dict2 = new Dictionary<string, ISet<string>>
            {
                {"foo", new HashSet<string>{"bar"}}
            };

            Assert.IsFalse(dict1.DeepEquals(dict2));
        }
    }
}

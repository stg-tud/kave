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

using System;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Collections
{
    [TestFixture]
    internal class WeakNameCacheTest
    {
        private int _numberOfFactoryCalls;
        private WeakNameCache<TestName> _uut;

        [SetUp]
        public void SetUp()
        {
            _numberOfFactoryCalls = 0;
            _uut = WeakNameCache<TestName>.Get(
                id =>
                {
                    _numberOfFactoryCalls++;
                    return new TestName {Identifier = id};
                });
        }

        [Test]
        public void EnsuresNameIdentity()
        {
            var instance1 = _uut.GetOrCreate(FreshKey('a'));
            GC.Collect();
            var instance2 = _uut.GetOrCreate(FreshKey('a'));

            Assert.AreSame(instance1, instance2);
        }

        // TODO discuss problem case
        [Test, Ignore("old solution (temporarily?) replaced")]
        public void EnsuresKeyIdentity()
        {
            var key = FreshKey('a');
            _uut.GetOrCreate(key);
            GC.Collect();
            _uut.GetOrCreate(key);

            // name was kept alive by strong reference to key
            Assert.AreEqual(1, _numberOfFactoryCalls);
        }

        [Test]
        public void EnsuresNameIdentityByKeyValueEquality()
        {
            var instance1 = _uut.GetOrCreate(FreshKey('a'));
            var instance2 = _uut.GetOrCreate(FreshKey('a'));

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void CreatesNamePerIdentifier()
        {
            var instance1 = _uut.GetOrCreate(FreshKey('a'));
            var instance2 = _uut.GetOrCreate(FreshKey('b'));

            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void RemovesNonReferencedNames()
        {
            _uut.GetOrCreate(FreshKey('a'));
            GC.Collect();
            _uut.GetOrCreate(FreshKey('a'));

            // recreates name on second access
            Assert.AreEqual(2, _numberOfFactoryCalls);
        }

        private string FreshKey(params char[] chars)
        {
            return new string(chars);
        }

        private class TestName : IName
        {
            public string Identifier { get; set; }

            public bool IsUnknown
            {
                get { return false; }
            }

            public bool IsHashed
            {
                get { return false; }
            }
        }
    }
}
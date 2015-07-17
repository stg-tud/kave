﻿/*
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

using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Collections
{
    [TestFixture]
    class WeakNameCacheTest
    {
        [Test]
        public void EnsuresObjectIdentity()
        {
            var cache1 = WeakNameCache<object>.Get(id => null);
            var cache2 = WeakNameCache<object>.Get(id => null);

            Assert.AreSame(cache1, cache2);
        }

        [Test]
        public void CreatesCachePerType()
        {
            var cache1 = WeakNameCache<object>.Get(id => null);
            var cache2 = WeakNameCache<string>.Get(id => null);

            Assert.AreNotSame(cache1, cache2);
        }
    }
}

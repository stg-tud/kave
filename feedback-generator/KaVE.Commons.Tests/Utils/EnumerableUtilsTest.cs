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
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    public class EnumerableUtilsTest
    {
        [Test]
        public void PrependWith()
        {
            var list = new List<int> {1, 2, 3};
            var en = list.PrependWith(0);

            CollectionAssert.AreEqual(new[] {0, 1, 2, 3}, en);
        }

        [Test]
        public void DoesNotPrependNull()
        {
            var list = new List<object> { new object(), new object(), new object() };
            var en = list.PrependWith(null);

            CollectionAssert.AreEqual(list, en);
        }
    }
}
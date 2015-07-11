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

using System.Collections;
using NUnit.Framework;

namespace KaVE.Commons.TestUtils
{
    public class ToStringAssert
    {
        public static void Reflection<T>(T obj)
        {
            var openingBrace = obj is IEnumerable ? "[" : "{";
            var expectedStart = string.Format("{0}@{1} {2}\n", typeof (T).Name, obj.GetHashCode(), openingBrace);
            var actual = obj.ToString();
            if (!actual.StartsWith(expectedStart))
            {
                Assert.Fail("unexpected ToString output: '{0}'\nexpected start: {1}", obj, expectedStart);
            }
        }
    }
}
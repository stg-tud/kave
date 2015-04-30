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
 *    - Dennis Albrecht
 *    - Sven Amann
 */

using System.Globalization;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    [TestFixture]
    internal class StringUtilsTest
    {
        [Test]
        public void SimpleRoundTrip()
        {
            const string expected = "asd";
            var actual = expected.AsBytes().AsString();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("myfoobar", true),
         TestCase("myFoobar", true),
         TestCase("myFOObar", true),
         TestCase("myfOObar", true),
         TestCase("myf00bar", false),
         TestCase("myfobar", false)]
        public void ContainsIgnoreCase(string value, bool expected)
        {
            Assert.AreEqual(expected, value.Contains("foo", CompareOptions.IgnoreCase));
        }

        [TestCase(new[] {"foo"}, true),
         TestCase(new[] {"wut"}, false),
         TestCase(new[] {"some", "other", "foo"}, true),
         TestCase(new[] {"but", "not", "this", "time"}, false)]
        public void ContainsAny(string[] needles, bool expected)
        {
            Assert.AreEqual(expected, "myfoobar".ContainsAny(needles));
        }
    }
}
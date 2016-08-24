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

using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    internal class CultureUtilsTest
    {
        [Test]
        public void German()
        {
            CultureUtils.SetCultureForThisThread("de-DE");
            var actual = "{0}".FormatEx(0.123);
            var expected = "0,123";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void English()
        {
            CultureUtils.SetCultureForThisThread("en-US");
            var actual = "{0}".FormatEx(0.123);
            var expected = "0.123";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Default()
        {
            CultureUtils.SetDefaultCultureForThisThread();
            var actual = "{0}".FormatEx(0.123);
            var expected = "0.123";
            Assert.AreEqual(expected, actual);
        }
    }
}
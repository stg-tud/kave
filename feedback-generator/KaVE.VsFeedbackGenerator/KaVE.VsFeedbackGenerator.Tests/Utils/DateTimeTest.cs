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
using System.Globalization;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    class DateTimeTest
    {
        [Test]
        public void DateTimeIsSerializedToShortDate()
        {
            var d = new DateTime(2013, 12, 6);
            var actual = d.ToString(CultureInfo.InvariantCulture);
            const string expected = "12/06/2013 00:00:00";
            Assert.AreEqual(expected, actual);
         }

        [Test]
        public void DateTimeCanBeReCreatedFromSerialization()
        {
            var d = new DateTime(2013, 12, 6);
            var tmp = d.ToString(CultureInfo.InvariantCulture);
            var actual = DateTime.Parse(tmp, CultureInfo.InvariantCulture);
            Assert.AreEqual(d, actual);
        }
    }
}

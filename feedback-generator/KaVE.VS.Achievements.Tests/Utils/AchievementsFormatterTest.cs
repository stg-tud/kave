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
using System.Numerics;
using KaVE.VS.Achievements.Utils;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Utils
{
    [TestFixture]
    public class AchievementsFormatterTest
    {
        [TestCase(1000, "1.000"), TestCase(10, "10"), TestCase(1000000000, "1.000.000.000"), TestCase(0, "0")]
        public void FormatIntTest(int number, string expected)
        {
            Assert.AreEqual(expected, number.Format());
        }

        private static readonly object[][] TimeSpanValues =
        {
            new object[]
            {
                new TimeSpan(0, 0, 0, 0, 200),
                "200 ms"
            },
            new object[]
            {
                new TimeSpan(0, 0, 0, 45),
                "45 s"
            },
            new object[]
            {
                new TimeSpan(0, 0, 30, 0),
                "30 min"
            },
            new object[]
            {
                new TimeSpan(0, 13, 0, 0),
                "13 hr"
            },
            new object[]
            {
                new TimeSpan(3, 16, 42, 45, 750),
                "3,696 d"
            }
        };

        [TestCaseSource("TimeSpanValues")]
        public void FormatTimeSpanTest(TimeSpan timeSpan, string expected)
        {
            Assert.AreEqual(expected, timeSpan.Format());
        }
    }
}
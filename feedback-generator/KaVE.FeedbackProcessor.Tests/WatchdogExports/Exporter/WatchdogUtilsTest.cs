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
using KaVE.FeedbackProcessor.WatchdogExports.Exporter;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Exporter
{
    internal class WatchdogUtilsTest
    {
        // Aug 27, 1982 - 12:34:56.789
        //   local: 399292496
        //   utc: 399299696

        // Mar 2, 1974 - 04:05:06.007
        //   local: 131425506007
        //   utc: 131429106007

        // May 6, 1995 - 07:08:09.000
        //   local: 799736889000
        //   utc: 799744089000

        // Jan 1, 1970 - 00:00:00.00
        //   local: -3600
        //   utc: 0

        private static readonly DateTimeKind[] DateTimeKinds =
        {
            DateTimeKind.Utc,
            DateTimeKind.Local,
            DateTimeKind.Unspecified
        };

        [Test]
        public void Winter()
        {
            foreach (var kind in DateTimeKinds)
            {
                AssertTimestamp(1974, 3, 2, 4, 5, 6, 7, kind, 131429106007L);
            }
        }

        [Test]
        public void Summer()
        {
            foreach (var kind in DateTimeKinds)
            {
                AssertTimestamp(1982, 8, 27, 12, 34, 56, 789, kind, 399299696789L);
            }
        }

        [Test]
        public void Min()
        {
            foreach (var kind in DateTimeKinds)
            {
                AssertTimestamp(1970, 1, 1, 0, 0, 0, 0, kind, 0L);
            }
        }

        [Test]
        public void TestWithDifferentWayOfConstruction()
        {
            AssertDateToTimestamp(
                DateTime.MinValue.AddYears(1994).AddMonths(4).AddDays(5).AddHours(7).AddMinutes(8).AddSeconds(9),
                799744089000L);
        }

        private static void AssertTimestamp(int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            int millis,
            DateTimeKind kind,
            long expectedTimestamp)
        {
            var date = new DateTime(year, month, day, hour, minute, second, millis, kind);
            AssertDateToTimestamp(date, expectedTimestamp);
        }

        private static void AssertDateToTimestamp(DateTime date,
            long expectedTimestamp)
        {
            Console.WriteLine(date);
            Console.WriteLine(date.Millisecond);
            var actualTimeStamp = date.AsUtcTimestamp();
            Assert.AreEqual(expectedTimestamp, actualTimeStamp);
        }
    }
}
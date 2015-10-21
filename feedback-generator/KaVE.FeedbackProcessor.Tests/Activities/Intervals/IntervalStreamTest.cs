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
using KaVE.FeedbackProcessor.Activities.Intervals;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.Intervals
{
    [TestFixture]
    class IntervalStreamTest
    {
        [Test]
        public void TracksWorkStart()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var uut = new IntervalStream<string>();
            uut.Append(new Interval<string>{Start = someDateTime});

            Assert.AreEqual(someDateTime, uut.Start);
        }

        [Test]
        public void TracksWorkEnd()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var uut = new IntervalStream<string>();
            uut.Append(new Interval<string> { End = someDateTime });
            
            Assert.AreEqual(someDateTime, uut.End);
        }

        [Test]
        public void TracksWorkTime()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var uut = new IntervalStream<string>();
            uut.Append(new Interval<string> { Start = someDateTime });
            uut.Append(new Interval<string> { End = someDateTime.AddHours(3)});

            Assert.AreEqual(TimeSpan.FromHours(3), uut.Duration);
        }
    }
}

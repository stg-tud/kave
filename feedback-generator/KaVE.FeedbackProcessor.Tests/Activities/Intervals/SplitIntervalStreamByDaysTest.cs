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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Utils.DateTime;
using KaVE.FeedbackProcessor.Activities.Intervals;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.Intervals
{
    [TestFixture]
    internal class SplitIntervalStreamByDaysTest
    {
        protected DateTime SomeDate { get; private set; }
        private readonly int _dayInSeconds = TimeSpan.FromDays(1).RoundedTotalSeconds();

        [SetUp]
        public void SetUp()
        {
            SomeDate = DateTimeFactory.SomeWorkingHoursDateTime();
        }

        [Test]
        public void KeepsIntervalsOnDayTogether()
        {
            var uut = Stream(Interval(0, "A", 10), Interval(20, "B", 1));

            var streams = uut.SplitByDay().ToList();

            Assert.AreEqual(new[] {uut}, streams);
            Assert.AreNotSame(uut, streams[0]);
        }

        [Test]
        public void SplitsOnDaybreak()
        {
            var uut = Stream(
                Interval(new DateTime(2015, 1, 1, 23, 59, 50), "A", new DateTime(2015, 1, 1, 23, 59, 59)),
                Interval(new DateTime(2015, 1, 2, 00, 00, 00), "B", new DateTime(2015, 1, 2, 00, 00, 01)),
                Interval(new DateTime(2015, 1, 2, 01, 00, 01), "C", new DateTime(2015, 1, 2, 01, 00, 02))
                );

            var streams = uut.SplitByDay();

            AssertStreams(
                streams,
                Stream(uut[0]),
                Stream(uut[1], uut[2]));
        }

        [Test]
        public void SplitsOnDaybreakWithOffset()
        {
            var uut = Stream(
                Interval(new DateTime(2015, 1, 1, 23, 59, 50), "A", new DateTime(2015, 1, 1, 23, 59, 59)),
                Interval(new DateTime(2015, 1, 2, 00, 00, 00), "B", new DateTime(2015, 1, 2, 00, 00, 01)),
                Interval(new DateTime(2015, 1, 2, 01, 00, 01), "C", new DateTime(2015, 1, 2, 01, 00, 02))
                );

            var streams = uut.SplitByDay(TimeSpan.FromHours(1));

            AssertStreams(
                streams,
                Stream(uut[0], uut[1]),
                Stream(uut[2]));
        }

        private void AssertStreams(IEnumerable<IntervalStream<string>> streams,
            params IntervalStream<string>[] expecteds)
        {
            Assert.AreEqual(expecteds, streams);
        }

        private static IntervalStream<string> Stream(params Interval<string>[] intervals)
        {
            return new IntervalStream<string>(intervals);
        }

        protected Interval<string> Interval(int startOffsetInSeconds,
            string id,
            int endOffsetInSeconds)
        {
            var start = SomeDate.AddSeconds(startOffsetInSeconds);
            var durationInSeconds = endOffsetInSeconds - startOffsetInSeconds;
            var duration = TimeSpan.FromSeconds(durationInSeconds);
            return new Interval<string>
            {
                Start = start,
                Id = id,
                End = start + duration
            };
        }

        protected Interval<T> Interval<T>(DateTime start,
            T activity,
            DateTime end)
        {
            return new Interval<T>
            {
                Start = start,
                Id = activity,
                End = end
            };
        }
    }
}
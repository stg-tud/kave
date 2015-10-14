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
using KaVE.FeedbackProcessor.Activities.Intervals;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.Intervals
{
    [TestFixture]
    class SplitIntervalStreamByDaysTest
    {
        protected DateTime SomeDateTime { get; private set; }

        [SetUp]
        public void SetUp()
        {
            SomeDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
        }

        [Test]
        public void KeepsIntervalsOnDayTogether()
        {
            var uut = new IntervalStream<string>();
            uut.Append(Interval(0, "A", 10));
            uut.Append(Interval(20, "B", 1));

            var streams = uut.SplitByDay();

            CollectionAssert.AreEqual(new[]{uut}, streams);
        }

        protected Interval<string> Interval(int startOffsetInSeconds,
            string id,
            int endOffsetInSeconds)
        {
            var start = SomeDateTime.AddSeconds(startOffsetInSeconds);
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

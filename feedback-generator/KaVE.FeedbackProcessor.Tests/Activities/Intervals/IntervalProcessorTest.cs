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
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.Intervals
{
    internal abstract class IntervalProcessorTest<TProcessor, TIntervalId>
        where TProcessor : IntervalProcessor<TIntervalId>
    {
        protected DateTime SomeDay { get { return SomeDateTime.Date; } }
        protected TimeSpan SomeTime { get { return SomeDateTime - SomeDay;  } }
        protected DateTime SomeDateTime { get; private set; }

        protected const int OneDay = 24*60*60;

        protected Developer SomeDeveloper { get; private set; }
        protected TProcessor Uut { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Uut = CreateProcessor();
            SomeDeveloper = TestFactory.SomeDeveloper();
            SomeDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
        }

        protected abstract TProcessor CreateProcessor();

        protected void WhenStreamIsProcessed(params ActivityEvent[] stream)
        {
            Uut.OnStreamStarts(SomeDeveloper);
            foreach (var @event in stream)
            {
                Uut.OnEvent(@event);
            }
            Uut.OnStreamEnds();
        }

        protected ActivityEvent SomeEvent(int offsetInSeconds, int durationInSeconds)
        {
            return new ActivityEvent
            {
                TriggeredAt = SomeDateTime.AddSeconds(offsetInSeconds),
                Duration = TimeSpan.FromSeconds(durationInSeconds)
            };
        }

        protected Interval<TIntervalId> Interval(int startOffsetInSeconds,
            TIntervalId id,
            int durationInSeconds)
        {
            var start = SomeDateTime.AddSeconds(startOffsetInSeconds);
            return new Interval<TIntervalId>
            {
                Start = start,
                Id = id,
                End = start + TimeSpan.FromSeconds(durationInSeconds)
            };
        }

        protected static Tuple<DateTime, IntervalStream<TIntervalId>> Stream(DateTime day,
            params Interval<TIntervalId>[] intervals)
        {
            return Tuple.Create(day, new IntervalStream<TIntervalId>(intervals));
        }

        protected void AssertStream(DateTime day, params Interval<TIntervalId>[] expecteds)
        {
            var actuals = Uut.Intervals[new DeveloperDay(SomeDeveloper, day)];
            Assert.AreEqual(expecteds, actuals);
        }

        protected void AssertStreams(params Tuple<DateTime, IntervalStream<TIntervalId>>[] streams)
        {
            AssertStreams(streams, Uut.Intervals);
        }

        protected void AssertStreams(IEnumerable<Tuple<DateTime, IntervalStream<TIntervalId>>> streams,
            IDictionary<DeveloperDay, IntervalStream<TIntervalId>> actuals)
        {
            var expecteds = new Dictionary<DeveloperDay, IntervalStream<TIntervalId>>();
            foreach (var stream in streams)
            {
                expecteds[new DeveloperDay(SomeDeveloper, stream.Item1)] = stream.Item2;
            }
            Assert.AreEqual(expecteds, actuals);
        }
    }
}
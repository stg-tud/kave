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
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.Intervals
{
    internal abstract class IntervalProcessorTest<TProcessor, TIntervalId> where TProcessor: IntervalProcessor<TIntervalId>
    {
        protected DateTime SomeDateTime { get; private set; }
        protected Developer SomeDeveloper { get; private set; }
        protected TProcessor Uut { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Uut = CreateProcessor();
            SomeDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            SomeDeveloper = TestFactory.SomeDeveloper();
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

        protected void AssertIntervals<T>(params Interval<T>[] expecteds)
        {
            var actuals = Uut.Intervals[SomeDeveloper];
            Assert.AreEqual(expecteds, actuals);
        }

        protected Interval<T> Interval<T>(int startOffsetInSeconds,
            T activity,
            int durationInSeconds)
        {
            var start = SomeDateTime.AddSeconds(startOffsetInSeconds);
            return new Interval<T>
            {
                Start = start,
                Id = activity,
                End = start + TimeSpan.FromSeconds(durationInSeconds)
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
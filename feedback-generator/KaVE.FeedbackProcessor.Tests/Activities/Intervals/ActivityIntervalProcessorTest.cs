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
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.Intervals
{
    [TestFixture]
    internal class ActivityIntervalProcessorTest : IntervalProcessorTest<ActivityIntervalProcessor, Activity>
    {
        protected override ActivityIntervalProcessor CreateProcessor()
        {
            return new ActivityIntervalProcessor();
        }

        [Test]
        public void Interval()
        {
            WhenStreamIsProcessed(SomeEvent(0, Activity.Other, 3));

            AssertStream(SomeDay, Interval(0, Activity.Other, 3));
        }

        [Test]
        public void IntervalWithoutEnd()
        {
            WhenStreamIsProcessed(SomeEvent(0, Activity.Other, 0));

            AssertStream(SomeDay, Interval(0, Activity.Other, 0));
        }

        [Test]
        public void Intervals()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Other, 1),
                SomeEvent(3, Activity.Navigation, 2));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Other, 1),
                Interval(3, Activity.Navigation, 2));
        }

        [Test]
        public void ProlongsInterval()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 1),
                SomeEvent(1, Activity.Development, 1));

            AssertStream(SomeDay, Interval(0, Activity.Development, 2));
        }

        [Test]
        public void IntervalsOfSameActivity()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Other, 1),
                SomeEvent(3, Activity.Other, 2));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Other, 1),
                Interval(3, Activity.Other, 2));
        }

        [Test(Description = "Ignore event duration, if another activity occurs concurrently.")]
        public void ConcurrentIntervals_VariantA()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.LocalConfiguration, 2),
                SomeEvent(1, Activity.LeaveIDE, 2));

            AssertStream(
                SomeDay,
                Interval(0, Activity.LocalConfiguration, 1),
                Interval(1, Activity.Away, 2));
        }

        [Test(Description = "Ignore new event, if previous event still endures"), Ignore]
        public void ConcurrentIntervals_VariantB()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.LocalConfiguration, 2),
                SomeEvent(1, Activity.LeaveIDE, 2));

            AssertStream(
                SomeDay,
                Interval(0, Activity.LocalConfiguration, 2),
                Interval(2, Activity.Away, 1));
        }

        [Test]
        public void ClosesIntervalGapsBelowTimeout()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Other, 1),
                SomeEvent(3, Activity.Navigation, 1));

            AssertCorrectedIntervals(
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(42),
                Interval(0, Activity.Other, 3),
                Interval(3, Activity.Navigation, 4));
        }

        [Test]
        public void MergesSameActivityIntervalsWithGapBelowInterval()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 1),
                SomeEvent(2, Activity.Development, 1),
                SomeEvent(4, Activity.Navigation, 1));

            AssertCorrectedIntervals(
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(42),
                Interval(0, Activity.Development, 4),
                Interval(4, Activity.Navigation, 3));
        }

        [Test]
        public void InsertsInactivityIfGapExceedsTimeout()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Other, 1),
                SomeEvent(3, Activity.Navigation, 1));

            AssertCorrectedIntervals(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(42),
                Interval(0, Activity.Other, 2),
                Interval(2, Activity.Inactive, 1),
                Interval(3, Activity.Navigation, 2));
        }

        [Test]
        public void InsertsLongInactivityIfGapExceedsThreshold()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Other, 1),
                SomeEvent(5, Activity.Navigation, 1));

            AssertCorrectedIntervals(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                Interval(0, Activity.Other, 2),
                Interval(2, Activity.InactiveLong, 3),
                Interval(5, Activity.Navigation, 2));
        }

        [Test]
        public void InsertsNothingIfInactivityUntilAnotherDay()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 1),
                SomeEvent(OneDay, Activity.Development, 1));

            AssertCorrectedStreams(
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(2),
                Stream(SomeDay, Interval(0, Activity.Development, 1)),
                Stream(SomeDay.AddDays(1), Interval(OneDay, Activity.Development, 1)));
        }

        [Test]
        public void InsertsAwayFromLeaveIDEToNextActivity()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.LeaveIDE, 1),
                SomeEvent(10, Activity.Development, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Away, 10),
                Interval(10, Activity.Development, 1));
        }

        [Test]
        public void InsertsNothingAfterLeaveIDEIfNextActivityIsOnAnotherDay()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.LeaveIDE, 1),
                SomeEvent(OneDay, Activity.Development, 1));

            AssertStreams(
                Stream(SomeDay, Interval(0, Activity.Away, 1)),
                Stream(SomeDay.AddDays(1), Interval(OneDay, Activity.Development, 1)));
        }

        [Test]
        public void InsertsAwayFromLastActivityUntilEnterIDE()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 1),
                SomeEvent(6, Activity.EnterIDE, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Development, 1),
                Interval(1, Activity.Away, 5),
                Interval(6, Activity.Other, 1));
        }

        [Test]
        public void InsertsNothingIfEnterIDEIsOnDifferentDayThanLastActivity()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 1),
                SomeEvent(OneDay, Activity.EnterIDE, 1));

            AssertStreams(
                Stream(SomeDay, Interval(0, Activity.Development, 1)),
                Stream(SomeDay.AddDays(1), Interval(OneDay, Activity.Other, 1)));
        }

        [Test]
        public void InsertsAwayBetweenEnterAndLeaveIDE()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.LeaveIDE, 1),
                SomeEvent(10, Activity.EnterIDE, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Away, 10),
                Interval(10, Activity.Other, 1));
        }

        [Test]
        public void InsertsNothingIfEnterAndLeaveIDEAreOnDifferentDays()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.LeaveIDE, 1),
                SomeEvent(OneDay, Activity.EnterIDE, 1));

            AssertStreams(
                Stream(SomeDay, Interval(0, Activity.Away, 1)),
                Stream(SomeDay.AddDays(1), Interval(OneDay, Activity.Other, 1)));
        }

        [Test]
        public void DoesNotInsertNegativeAwayInterval()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 2),
                SomeEvent(1, Activity.EnterIDE, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Development, 1),
                Interval(1, Activity.Other, 1));
        }

        [Test]
        public void AnyActivityIsMappedToOther()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 1),
                SomeEvent(2, Activity.Any, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Development, 1),
                Interval(2, Activity.Other, 1));
        }

        [Test]
        public void AnyActivityStartsOtherInterval()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Any, 1));

            AssertStream(SomeDay, Interval(0, Activity.Other, 1));
        }

        [Test]
        public void AnyDoesNotShortenInterval()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Development, 3),
                SomeEvent(0, Activity.Any, 0));

            AssertStream(SomeDay, Interval(0, Activity.Development, 3));
        }

        [Test]
        public void IgnoresAnyIfConcurrentToSpecificActivity()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Any, 0),
                SomeEvent(0, Activity.Development, 1));

            AssertStream(SomeDay, Interval(0, Activity.Development, 1));
        }

        [Test]
        public void IgnoresAnyIfConcurrentToSpecificActivityAfterActivity()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Navigation, 1),
                SomeEvent(2, Activity.Any, 1),
                SomeEvent(2, Activity.Development, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Navigation, 1),
                Interval(2, Activity.Development, 1));
        }

        [Test]
        public void EnsuresSequentialIntervalsWhileWaiting_1()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Waiting, 5),
                SomeEvent(3, Activity.Development, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Waiting, 3),
                Interval(3, Activity.Development, 1));
        }

        [Test]
        public void EnsuresSequentialIntervalsWhileWaiting_2()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Waiting, 5),
                SomeEvent(3, Activity.Development, 6));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Waiting, 3),
                Interval(3, Activity.Development, 6));
        }

        [Test]
        public void AnyAndSpecificActivityWhileWaiting()
        {
            WhenStreamIsProcessed(
                SomeEvent(0, Activity.Waiting, 3),
                SomeEvent(1, Activity.Any, 1),
                SomeEvent(1, Activity.Development, 1));

            AssertStream(
                SomeDay,
                Interval(0, Activity.Waiting, 1),
                Interval(1, Activity.Development, 1));
        }

        private ActivityEvent SomeEvent(int offsetInSeconds, Activity activity, int durationInSeconds)
        {
            var activityEvent = SomeEvent(offsetInSeconds, durationInSeconds);
            activityEvent.Activity = activity;
            return activityEvent;
        }

        private void AssertCorrectedIntervals<T>(TimeSpan activityTimeout,
            TimeSpan shortInactivityTimeout,
            params Interval<T>[] expecteds)
        {
            var correctedIntervals = Uut.GetIntervalsWithCorrectTimeouts(activityTimeout, shortInactivityTimeout);
            var actuals = correctedIntervals[new DeveloperDay(SomeDeveloper, SomeDay)];
            Assert.AreEqual(expecteds, actuals);
        }

        private void AssertCorrectedStreams(TimeSpan activityTimeout,
            TimeSpan shortInactivityTimeout,
            params Tuple<DateTime, IntervalStream<Activity>>[] streams)
        {
            AssertStreams(streams, Uut.GetIntervalsWithCorrectTimeouts(activityTimeout, shortInactivityTimeout));
        }
    }
}
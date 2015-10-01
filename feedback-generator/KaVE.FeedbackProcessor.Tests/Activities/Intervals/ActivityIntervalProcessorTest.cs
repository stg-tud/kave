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
    [TestFixture]
    class ActivityIntervalProcessorTest
    {
        private ActivityIntervalProcessor _uut;
        private DateTime _someDateTime;
        private Developer _someDeveloper;

        [SetUp]
        public void SetUp()
        {
            _uut = new ActivityIntervalProcessor();
            _someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            _someDeveloper = TestFactory.SomeDeveloper();
        }

        [Test]
        public void Interval()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.Other));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(3), Activity.Other, 1));
            _uut.OnStreamEnds();

            AssertIntervals(_someDeveloper,
                Interval(_someDateTime, Activity.Other, 4));
        }

        [Test]
        public void IntervalWithoutEnd()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.Other));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(3), Activity.Other));
            _uut.OnStreamEnds();

            AssertIntervals(_someDeveloper,
                Interval(_someDateTime, Activity.Other, 3));
        }

        [Test]
        public void Intervals()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.Other,1));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(3), Activity.Navigation, 2));
            _uut.OnStreamEnds();

            AssertIntervals(_someDeveloper,
                Interval(_someDateTime, Activity.Other, 1),
                Interval(_someDateTime.AddSeconds(3), Activity.Navigation, 2));
        }

        [Test]
        public void ClosesIntervalGapsBelowTimeout()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.Other, 1));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(3), Activity.Navigation, 1));
            _uut.OnStreamEnds();

            _uut.CorrectIntervalsWithTimeout(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(42));

            AssertIntervals(_someDeveloper,
                Interval(_someDateTime, Activity.Other, 3),
                Interval(_someDateTime.AddSeconds(3), Activity.Navigation, 1));
        }

        [Test]
        public void InsertsInactivityIfGapExceedsTimeout()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.Other, 1));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(3), Activity.Navigation, 1));
            _uut.OnStreamEnds();

            _uut.CorrectIntervalsWithTimeout(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(42));

            AssertIntervals(
                _someDeveloper,
                Interval(_someDateTime, Activity.Other, 2),
                Interval(_someDateTime.AddSeconds(2), Activity.Inactive, 1),
                Interval(_someDateTime.AddSeconds(3), Activity.Navigation, 1));
        }

        [Test]
        public void InsertsLongInactivityIfGapExceedsThreshold()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.Other, 1));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(5), Activity.Navigation, 1));
            _uut.OnStreamEnds();

            _uut.CorrectIntervalsWithTimeout(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));

            AssertIntervals(
                _someDeveloper,
                Interval(_someDateTime, Activity.Other, 2),
                Interval(_someDateTime.AddSeconds(2), Activity.InactiveLong, 3),
                Interval(_someDateTime.AddSeconds(5), Activity.Navigation, 1));
        }

        [Test]
        public void InsertsAwayFromLeaveIDEToNextActivity()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.LeaveIDE, 1));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(10), Activity.Development, 1));
            _uut.OnStreamEnds();

            AssertIntervals(_someDeveloper,
                Interval(_someDateTime, Activity.Away, 10),
                Interval(_someDateTime.AddSeconds(10), Activity.Development, 1));
        }

        [Test]
        public void InsertsAwayFromLastActivityUntilEnterIDE()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.Development, 1));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(6), Activity.EnterIDE, 1));
            _uut.OnStreamEnds();

            AssertIntervals(_someDeveloper,
                Interval(_someDateTime, Activity.Development, 1),
                Interval(_someDateTime.AddSeconds(1), Activity.Away, 5),
                Interval(_someDateTime.AddSeconds(6), Activity.Other, 1));
        }

        [Test]
        public void InsertsAwayBetweenEnterAndLeaveIDE()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime, Activity.LeaveIDE, 1));
            _uut.OnEvent(SomeEvent(_someDateTime.AddSeconds(10), Activity.EnterIDE, 1));
            _uut.OnStreamEnds();

            AssertIntervals(_someDeveloper,
                Interval(_someDateTime, Activity.Away, 10),
                Interval(_someDateTime.AddSeconds(10), Activity.Other, 1));
        }

        private void AssertIntervals(Developer developer, params ActivityIntervalProcessor.Interval[] expecteds)
        {
            var actuals = _uut.Intervals[developer];
            Assert.AreEqual(expecteds, actuals);
        }

        private static ActivityIntervalProcessor.Interval Interval(DateTime start, Activity activity, int durationInSeconds)
        {
            return new ActivityIntervalProcessor.Interval
            {
                Start = start,
                Activity = activity,
                End = start + TimeSpan.FromSeconds(durationInSeconds)
            };
        }

        private static ActivityEvent SomeEvent(DateTime triggeredAt, Activity activity)
        {
            return new ActivityEvent { TriggeredAt = triggeredAt, Activity = activity};
        }

        private static ActivityEvent SomeEvent(DateTime triggeredAt, Activity activity, int durationInSeconds)
        {
            return new ActivityEvent { TriggeredAt = triggeredAt, Activity = activity, Duration = TimeSpan.FromSeconds(durationInSeconds) };
        }
    }
}

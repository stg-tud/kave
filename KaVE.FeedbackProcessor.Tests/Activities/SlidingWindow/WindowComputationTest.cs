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
using KaVE.Commons.Utils.DateTimes;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal class WindowComputationTest
    {
        private static readonly TimeSpan WindowSpan = TimeSpan.FromSeconds(1);

        private ActivityWindowProcessor _uut;
        private TestMergeStrategy _testMergeStrategy;
        private Developer _someDeveloper;
        private DateTime _someDateTime;

        [SetUp]
        public void SetUp()
        {
            _testMergeStrategy = new TestMergeStrategy();
            _uut = new ActivityWindowProcessor(_testMergeStrategy, WindowSpan);

            _someDeveloper = TestFactory.SomeDeveloper();
            _someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
        }

        [Test]
        public void EndsWindowOnFirstEventAfterWindowEnd()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan);

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);

            AssertWindows(WindowFrom(event1));
        }

        [Test]
        public void EndsWindowOnStreamEnd()
        {
            var event1 = SomeEvent(_someDateTime);

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnStreamEnds();

            AssertWindows(WindowFrom(event1));
        }

        [Test]
        public void ComputesSubsequentWindow()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan);
            var event3 = SomeEvent(_someDateTime + WindowSpan + WindowSpan);

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnEvent(event3);

            AssertWindows(WindowFrom(event1), WindowFrom(event2));
        }

        [Test]
        public void ComputesWindowOnNextDay()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime.AddDays(1));
            var event3 = SomeEvent(_someDateTime.AddDays(1).AddMilliseconds(1));

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnEvent(event3);
            _uut.OnStreamEnds();

            AssertWindows(WindowFrom(event1), WindowFrom(event2, event3));
        }

        [Test]
        public void ComputesEmptyWindows()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan + WindowSpan);

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);

            AssertWindows(WindowFrom(event1), EmptyWindow(_someDateTime + WindowSpan));
        }

        [Test]
        public void ComputesNoEmptyWindowsBetweenLastEventOfDayAndNextEvent()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime.AddDays(1));

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);

            AssertWindows(WindowFrom(event1));
        }

        [Test]
        public void ComputesNotWindowsForEmptyStream()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnStreamEnds();

            AssertWindows( /* none */);
        }

        [Test]
        public void ResetsMergeStrategyForNextDeveloper()
        {
            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(SomeEvent(_someDateTime));
            _uut.OnEvent(SomeEvent(_someDateTime.AddMinutes(1)));
            _uut.OnStreamEnds();

            Assert.AreEqual(1, _testMergeStrategy.NumberOfResets);
        }

        [Test]
        public void ResetsMergeStrategyForNextDay()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime.AddDays(1));

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            var expected = _testMergeStrategy.NumberOfResets + 1;
            _uut.OnEvent(event2);

            Assert.AreEqual(expected, _testMergeStrategy.NumberOfResets);
        }

        [Test]
        public void SplitsLongEvent()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan.Times(0.5), WindowSpan);

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnStreamEnds();

            var event2P1 = SomeEvent(event2.GetTriggeredAt(), WindowSpan.Times(0.5));
            var event2P2 = SomeEvent(_someDateTime + WindowSpan, WindowSpan.Times(0.5));
            AssertWindows(WindowFrom(event1, event2P1), WindowFrom(event2P2));
        }

        [Test]
        public void SplitsLongEventToMultipleWindows()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan.Times(0.5), WindowSpan.Times(2.5));

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnStreamEnds();

            var event2P1 = SomeEvent(event2.GetTriggeredAt(), WindowSpan.Times(0.5));
            var event2P2 = SomeEvent(_someDateTime + WindowSpan, WindowSpan);
            var event2P3 = SomeEvent(_someDateTime + WindowSpan + WindowSpan, WindowSpan);
            AssertWindows(WindowFrom(event1, event2P1), WindowFrom(event2P2), WindowFrom(event2P3));
        }

        [Test]
        public void AssignsParrallelEventToRightWindow()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan.Times(0.5), WindowSpan.Times(2.5));
            var parallelEvent = SomeEvent(_someDateTime + WindowSpan.Times(0.7));

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnEvent(parallelEvent);
            _uut.OnStreamEnds();

            var event2P1 = SomeEvent(event2.GetTriggeredAt(), WindowSpan.Times(0.5));
            var event2P2 = SomeEvent(_someDateTime + WindowSpan, WindowSpan);
            var event2P3 = SomeEvent(_someDateTime + WindowSpan + WindowSpan, WindowSpan);
            AssertWindows(WindowFrom(event1, event2P1, parallelEvent), WindowFrom(event2P2), WindowFrom(event2P3));
        }

        [Test]
        public void SplitsParallelEventToRightWindows()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan.Times(0.5), WindowSpan.Times(2.5));
            var parallelEvent = SomeEvent(_someDateTime + WindowSpan.Times(0.7), WindowSpan.Times(0.6));

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnEvent(parallelEvent);
            _uut.OnStreamEnds();

            var event2P1 = SomeEvent(event2.GetTriggeredAt(), WindowSpan.Times(0.5));
            var event2P2 = SomeEvent(_someDateTime + WindowSpan, WindowSpan);
            var event2P3 = SomeEvent(_someDateTime + WindowSpan + WindowSpan, WindowSpan);
            var parallelEventP1 = SomeEvent(parallelEvent.GetTriggeredAt(), WindowSpan.Times(0.3));
            var parallelEventP2 = SomeEvent(_someDateTime + WindowSpan, WindowSpan.Times(0.3));
            AssertWindows(
                WindowFrom(event1, event2P1, parallelEventP1),
                WindowFrom(event2P2, parallelEventP2),
                WindowFrom(event2P3));
        }

        [Test]
        public void SplitsAndAssignsMultipleParallelEventsRight()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan.Times(0.5), WindowSpan);
            var event3 = SomeEvent(_someDateTime + WindowSpan.Times(0.7), WindowSpan);
            var event4 = SomeEvent(_someDateTime + WindowSpan.Times(1.8));

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnEvent(event3);
            _uut.OnEvent(event4);
            _uut.OnStreamEnds();

            var event2P1 = SomeEvent(event2.GetTriggeredAt(), WindowSpan.Times(0.5));
            var event2P2 = SomeEvent(_someDateTime + WindowSpan, WindowSpan.Times(0.5));
            var event3P1 = SomeEvent(event3.GetTriggeredAt(), WindowSpan.Times(0.3));
            var event3P2 = SomeEvent(_someDateTime + WindowSpan, WindowSpan.Times(0.7));
            AssertWindows(WindowFrom(event1, event2P1, event3P1), WindowFrom(event2P2, event3P2, event4));
        }

        [Test]
        public void DoesNotModifyOriginalEventWhenSplitting()
        {
            var event1 = SomeEvent(_someDateTime);
            var event2 = SomeEvent(_someDateTime + WindowSpan.Times(0.5), WindowSpan);

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnStreamEnds();

            Assert.AreEqual(event2.Duration, WindowSpan);
        }

        private static ActivityEvent SomeEvent(DateTime triggeredAt)
        {
            return new ActivityEvent {TriggeredAt = triggeredAt};
        }

        private static ActivityEvent SomeEvent(DateTime triggeredAt, TimeSpan duration)
        {
            return new ActivityEvent {TriggeredAt = triggeredAt, Duration = duration};
        }

        private static Window EmptyWindow(DateTime windowStart)
        {
            return new Window(windowStart, WindowSpan);
        }

        private static Window WindowFrom(params ActivityEvent[] events)
        {
            var window = new Window(events[0].GetTriggeredAt(), WindowSpan);
            foreach (var activityEvent in events)
            {
                window.Add(activityEvent);
            }
            return window;
        }

        private void AssertWindows(params Window[] expecteds)
        {
            var actuals = _testMergeStrategy.Windows;
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        private class TestMergeStrategy : IActivityMergeStrategy
        {
            public readonly IList<Window> Windows = new List<Window>();
            public int NumberOfResets { get; private set; }

            public Activity Merge(Window window)
            {
                Windows.Add(window);
                return Activity.Any;
            }

            public void Reset()
            {
                NumberOfResets++;
            }
        }
    }
}
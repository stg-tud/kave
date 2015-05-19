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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
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
            _someDateTime = DateTimeFactory.SomeDateTime();
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

            AssertWindows(WindowFrom(event1), EmptyWindow());
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

        private static ActivityEvent SomeEvent(DateTime triggeredAt)
        {
            return new ActivityEvent {TriggeredAt = triggeredAt};
        }

        private static Activity[] EmptyWindow()
        {
            return WindowFrom();
        }

        private static Activity[] WindowFrom(params ActivityEvent[] events)
        {
            return events.Select(e => e.Activity).ToArray();
        }

        private void AssertWindows(params Activity[][] expecteds)
        {
            var actuals = _testMergeStrategy.Windows;
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        private class TestMergeStrategy : ActivityWindowProcessor.IActivityMergeStrategy
        {
            public readonly IList<IList<Activity>> Windows = new List<IList<Activity>>();

            public Activity Merge(IList<Activity> window)
            {
                Windows.Add(window);
                return Activity.Any;
            }
        }
    }
}
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
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal class ActivityStreamCreationTest
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
        public void StoresStreamForDeveloper()
        {
            var event1 = new ActivityEvent {TriggeredAt = _someDateTime};
            var event2 = new ActivityEvent {TriggeredAt = _someDateTime + WindowSpan};
            var event3 = new ActivityEvent {TriggeredAt = _someDateTime + WindowSpan + WindowSpan};

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnEvent(event3);
            _uut.OnStreamEnds();

            CollectionAssert.AreEqual(
                new[] {Activity.Away, Activity.Away, Activity.Away},
                _uut.ActivityStream[_someDeveloper][_someDateTime.Date]);
        }

        [Test]
        public void SeparatesStreamsForDevelopers()
        {
            var event1 = new ActivityEvent {TriggeredAt = _someDateTime};
            var event2 = new ActivityEvent {TriggeredAt = _someDateTime + WindowSpan};
            var event3 = new ActivityEvent {TriggeredAt = _someDateTime + WindowSpan + WindowSpan};

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnStreamEnds();
            var otherDeveloper = TestFactory.SomeDeveloper();
            _uut.OnStreamStarts(otherDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnEvent(event3);
            _uut.OnStreamEnds();

            CollectionAssert.AreEqual(new[] { Activity.Away, Activity.Away }, _uut.ActivityStream[_someDeveloper][_someDateTime.Date]);
            CollectionAssert.AreEqual(
                new[] {Activity.Away, Activity.Away, Activity.Away},
                _uut.ActivityStream[otherDeveloper][_someDateTime.Date]);
        }

        [Test]
        public void SplitsStreamsByDay()
        {
            var event1 = new ActivityEvent {TriggeredAt = _someDateTime};
            var event2 = new ActivityEvent {TriggeredAt = _someDateTime.AddDays(1)};

            _uut.OnStreamStarts(_someDeveloper);
            _uut.OnEvent(event1);
            _uut.OnEvent(event2);
            _uut.OnStreamEnds();

            var expected = new Dictionary<DateTime, IList<Activity>>
            {
                {event1.GetTriggerDate(), new List<Activity> {Activity.Away}},
                {event2.GetTriggerDate(), new List<Activity> {Activity.Away}}
            };

            CollectionAssert.AreEqual(expected, _uut.ActivityStream[_someDeveloper]);
        }

        private class TestMergeStrategy : ActivityWindowProcessor.IActivityMergeStrategy
        {
            public Activity Merge(IList<Activity> window)
            {
                return Activity.Away;
            }
        }
    }
}
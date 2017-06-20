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
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal class CreateActivityStreamTest
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

            WhenStreamIsProcessed(_someDeveloper, event1, event2, event3);

            AssertActivityStream(_someDeveloper, _someDateTime.Date, Activity.Away, Activity.Away, Activity.Away);
        }

        [Test]
        public void SeparatesStreamsForDevelopers()
        {
            var event1 = new ActivityEvent {TriggeredAt = _someDateTime};
            var event2 = new ActivityEvent {TriggeredAt = _someDateTime + WindowSpan};
            var event3 = new ActivityEvent {TriggeredAt = _someDateTime + WindowSpan + WindowSpan};

            WhenStreamIsProcessed(_someDeveloper, event1, event2);
            var otherDeveloper = TestFactory.SomeDeveloper();
            WhenStreamIsProcessed(otherDeveloper, event1, event2, event3);

            AssertActivityStream(_someDeveloper, _someDateTime.Date, Activity.Away, Activity.Away);
            AssertActivityStream(otherDeveloper, _someDateTime.Date, Activity.Away, Activity.Away, Activity.Away);
        }

        [Test]
        public void SplitsStreamsByDay()
        {
            var event1 = new ActivityEvent {TriggeredAt = _someDateTime};
            var event2 = new ActivityEvent {TriggeredAt = _someDateTime.AddDays(1)};

            WhenStreamIsProcessed(_someDeveloper, event1, event2);

            AssertActivityStream(_someDeveloper, event1.GetTriggerDate(), Activity.Away);
            AssertActivityStream(_someDeveloper, event2.GetTriggerDate(), Activity.Away);
        }

        private void WhenStreamIsProcessed(Developer developer, params ActivityEvent[] events)
        {
            _uut.OnStreamStarts(developer);
            foreach (var @event in events)
            {
                _uut.OnEvent(@event);
            }
            _uut.OnStreamEnds();
        }

        private void AssertActivityStream(Developer developer, DateTime day, params Activity[] expectedStream)
        {
            CollectionAssert.AreEqual(expectedStream, _uut.ActivityStreams[developer][day]);
        }

        private class TestMergeStrategy : IActivityMergeStrategy
        {
            public Activity Merge(Window window)
            {
                return Activity.Away;
            }

            public void Reset() {}
        }
    }
}
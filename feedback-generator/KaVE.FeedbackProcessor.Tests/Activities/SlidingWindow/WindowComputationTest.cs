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
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal class WindowComputationTest
    {
        [Test]
        public void ComputesWindowFromStream()
        {
            var someDateTime = DateTimeFactory.SomeDateTime();
            var event1 = GivenEventExists(someDateTime);
            var event2 = GivenEventExists(someDateTime.AddSeconds(1));
            var event3 = GivenEventExists(someDateTime.AddSeconds(2));
            var event4 = GivenEventExists(someDateTime.AddSeconds(3));

            var testMergeStrategy = new TestMergeStrategy();
            var uut = new ActivityWindowProcessor(testMergeStrategy, TimeSpan.FromSeconds(3));
            uut.OnEvent(event1);
            uut.OnEvent(event2);
            uut.OnEvent(event3);
            uut.OnEvent(event4);
            var window = testMergeStrategy.Windows.First();

            CollectionAssert.AreEqual(ToActivities(event1, event2, event3), window);
        }

        private static ActivityEvent GivenEventExists(DateTime triggeredAt)
        {
            return new ActivityEvent {TriggeredAt = triggeredAt};
        }

        private static IEnumerable<Activity> ToActivities(params ActivityEvent[] events)
        {
            return events.Select(e => e.Activity);
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
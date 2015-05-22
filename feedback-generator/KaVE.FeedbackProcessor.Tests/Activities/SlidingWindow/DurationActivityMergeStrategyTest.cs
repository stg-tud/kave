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
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal class DurationActivityMergeStrategyTest : WeightedActivityMergeStrategyContract
    {
        protected override IActivityMergeStrategy CreateStrategy()
        {
            return new DurationActivityMergingStrategy();
        }

        [Test]
        public void SelectsLongestActivity()
        {
            var actual = Strategy.Merge(Window(Event(Activity.Development, 50), Event(Activity.Navigation, 10)));

            Assert.AreEqual(Activity.Development, actual);
        }

        [Test]
        public void SelectsLongestActivityInTotal()
        {
            var actual =
                Strategy.Merge(
                    Window(
                        Event(Activity.Development, 5),
                        Event(Activity.Navigation, 10),
                        Event(Activity.Development, 8)));

            Assert.AreEqual(Activity.Development, actual);
        }

        private static IList<ActivityEvent> Window(params ActivityEvent[] activityEvents)
        {
            return activityEvents;
        }

        private static ActivityEvent Event(Activity activity, int durationInMilliseconds)
        {
            return new ActivityEvent
            {
                Activity = activity,
                TriggeredAt = DateTimeFactory.SomeWorkingHoursDateTime(),
                Duration = TimeSpan.FromMilliseconds(durationInMilliseconds)
            };
        }
    }
}
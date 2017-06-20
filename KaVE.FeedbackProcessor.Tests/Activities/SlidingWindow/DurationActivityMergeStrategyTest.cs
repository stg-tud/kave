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
            var actual = Strategy.Merge(Window(Event(Activity.Development, 1000), Event(Activity.Navigation, 900)));

            Assert.AreEqual(Activity.Development, actual);
        }

        [Test]
        public void SelectsLongestActivityInTotal()
        {
            var actual =
                Strategy.Merge(
                    Window(
                        Event(Activity.Development, 800),
                        Event(Activity.Navigation, 1000),
                        Event(Activity.Development, 900)));

            Assert.AreEqual(Activity.Development, actual);
        }

        [Test(Description = "We assume that every activity de facto needs some minimal amount of a developer's time")]
        public void UsesMinimalDurationForEventsWithoutDuration()
        {
            var actual = Strategy.Merge(
                Window(
                    Event(Activity.Navigation, 0),
                    Event(Activity.Navigation, 0),
                    Event(Activity.Other, 200),
                    Event(Activity.Navigation, 0)));

            Assert.AreEqual(Activity.Navigation, actual);
        }

        [Test]
        public void SelectsLaterActivityOnEqualDuration()
        {
            var actual = Strategy.Merge(Window(Activity.Development, Activity.Navigation));

            Assert.AreEqual(Activity.Navigation, actual);
        }

        private static Window Window(params ActivityEvent[] activityEvents)
        {
            var window = new Window(DateTimeFactory.SomeWorkingHoursDateTime(), TimeSpan.FromSeconds(5));
            foreach (var activityEvent in activityEvents)
            {
                window.Add(activityEvent);
            }
            return window;
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
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
using System.Linq;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal abstract class WeightedActivityMergeStrategyContract
    {
        protected IActivityMergeStrategy Strategy { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Strategy = CreateStrategy();
        }

        protected abstract IActivityMergeStrategy CreateStrategy();

        [Test]
        public void MapEmptyWindowToInactive()
        {
            var actual = Strategy.Merge(EmptyWindow());

            Assert.AreEqual(Activity.Inactive, actual);
        }

        [Test]
        public void MapsEmptyWindowToAwayAfterLeaveIDEOccurance()
        {
            Strategy.Merge(Window(Activity.LeaveIDE));
            var actual = Strategy.Merge(EmptyWindow());

            Assert.AreEqual(Activity.Away, actual);
        }

        [Test]
        public void AssumesBackInIDEIfAnyActivityOccurs()
        {
            Strategy.Merge(Window(Activity.LeaveIDE));
            Strategy.Merge(Window(Activity.Any));
            var actual = Strategy.Merge(EmptyWindow());

            Assert.AreEqual(Activity.Inactive, actual);
        }

        [TestCase(Activity.Any),
         TestCase(Activity.EnterIDE),
         TestCase(Activity.LeaveIDE)]
        public void IgnoresSpecialActivity(Activity specialActivity)
        {
            var actual = Strategy.Merge(Window(specialActivity, specialActivity, Activity.Navigation));

            Assert.AreEqual(Activity.Navigation, actual);
        }

        [TestCase(Activity.Any),
         TestCase(Activity.EnterIDE),
         TestCase(Activity.LeaveIDE)]
        public void MapsOnlySpecialActivityToOtherByDefault(Activity specialActivity)
        {
            var actual = Strategy.Merge(Window(specialActivity));

            Assert.AreEqual(Activity.Other, actual);
        }

        [TestCase(Activity.Any),
         TestCase(Activity.EnterIDE),
         TestCase(Activity.LeaveIDE)]
        public void MapsOnlySpecialActivityToPrevious(Activity specialActivity)
        {
            var previous = Strategy.Merge(Window(Activity.Development));
            var actual = Strategy.Merge(Window(specialActivity));

            Assert.AreEqual(previous, actual);
        }

        [Test]
        public void ClearsPreviousOnReset()
        {
            Strategy.Merge(Window(Activity.Development));
            Strategy.Reset();
            var actual = Strategy.Merge(Window(Activity.Any));

            Assert.AreEqual(Activity.Other, actual);
        }

        [Test]
        public void ClearsOutsideIDEOnReset()
        {
            Strategy.Merge(Window(Activity.LeaveIDE));
            Strategy.Reset();
            var actual = Strategy.Merge(EmptyWindow());

            Assert.AreEqual(Activity.Inactive, actual);
        }

        [TestCase(Activity.Inactive), TestCase(Activity.Away)]
        public void MapsOnlyAnyToOtherIfPreviousIsInactivity(Activity inactivity)
        {
            Strategy.Merge(Window(inactivity));
            var actual = Strategy.Merge(Window(Activity.Any));

            Assert.AreEqual(Activity.Other, actual);
        }

        protected static Window EmptyWindow()
        {
            return Window( /* no activities */);
        }

        protected static Window Window(params Activity[] activities)
        {
            var window = new Window(DateTimeFactory.SomeWorkingHoursDateTime(), TimeSpan.FromSeconds(5));
            foreach (var activityEvent in activities.Select(a => new ActivityEvent {Activity = a}))
            {
                window.Add(activityEvent);
            }
            return window;
        }
    }
}
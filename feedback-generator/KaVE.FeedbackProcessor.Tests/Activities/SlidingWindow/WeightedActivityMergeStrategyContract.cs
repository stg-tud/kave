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
        public void ReturnsWaitingForEmptyWindow()
        {
            var actual = Strategy.Merge(EmptyWindow());

            Assert.AreEqual(Activity.Waiting, actual);
        }

        [Test]
        public void IgnoresAnyActivity()
        {
            var actual = Strategy.Merge(Window(Activity.Any, Activity.Any, Activity.Navigation));

            Assert.AreEqual(Activity.Navigation, actual);
        }

        [Test]
        public void MapsOnlyAnyToOtherByDefault()
        {
            var actual = Strategy.Merge(Window(Activity.Any));

            Assert.AreEqual(Activity.Other, actual);
        }

        [Test]
        public void MapsOnlyAnyToPrevious()
        {
            var previous = Strategy.Merge(Window(Activity.Development));
            var actual = Strategy.Merge(Window(Activity.Any));

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

        [TestCase(Activity.Waiting), TestCase(Activity.Away)]
        public void MapsOnlyAnyToOtherIfPreviousIsWaitingOrAway(Activity waitingOrAway)
        {
            Strategy.Merge(Window(waitingOrAway));
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
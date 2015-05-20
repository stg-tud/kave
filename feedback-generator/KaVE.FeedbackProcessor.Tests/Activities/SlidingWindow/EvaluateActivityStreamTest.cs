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
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Activities.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    class EvaluateActivityStreamTest
    {
        private static readonly TimeSpan WindowSpan = TimeSpan.FromSeconds(2);

        [Test]
        public void CountsOccurrancesOfActivityTimesWindowSpan()
        {
            var uut = Stream(Activity.Development, Activity.Development, Activity.Development);

            var statistic = uut.Evaluate(WindowSpan);

            Assert.AreEqual(statistic[Activity.Development], Times(WindowSpan,3));
        }

        private static ActivityStream Stream(params Activity[] activities)
        {
            return new ActivityStream(activities);
        }

        public static TimeSpan Times(TimeSpan span, int factor)
        {
            return TimeSpan.FromTicks(factor*span.Ticks);
        }

        private class ActivityStream
        {
            public ActivityStream(IList<Activity> activities)
            {
                Activities = activities;
            }

            public IList<Activity> Activities { get; private set; }

            public IDictionary<Activity, TimeSpan> Evaluate(TimeSpan windowSpan)
            {
                var activities = new Multiset<Activity>(Activities);
                var statistic = new Dictionary<Activity, TimeSpan>();
                foreach (var kvp in activities.EntryDictionary)
                {
                    statistic[kvp.Key] = Times(windowSpan, kvp.Value);
                }
                return statistic;
            }
        }
    }
}

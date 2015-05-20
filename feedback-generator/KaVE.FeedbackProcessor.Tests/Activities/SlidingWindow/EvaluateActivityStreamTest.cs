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
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal class EvaluateActivityStreamTest
    {
        private static readonly TimeSpan WindowSpan = TimeSpan.FromSeconds(2);

        [Test]
        public void CountsOccurrancesOfActivityTimesWindowSpan()
        {
            var uut = Stream(Activity.Development, Activity.Development, Activity.Development);

            var statistic = uut.Evaluate(TimeSpan.MaxValue);

            Assert.AreEqual(Times(WindowSpan, 3), statistic[Activity.Development]);
        }

        [Test]
        public void CountsShortWaitingPeriod()
        {
            var uut = Stream(Activity.Waiting, Activity.Waiting);

            var statistic = uut.Evaluate(Times(WindowSpan, 3));

            Assert.AreEqual(Times(WindowSpan, 2), statistic[Activity.Waiting]);
        }

        [Test(Description = "A Waiting window can only occur in between other windows (see WindowComputationTest)")]
        public void CountsAwayIfWaitingPeriodExceedsThreshold()
        {
            var uut = Stream(Activity.Any, Activity.Waiting, Activity.Waiting, Activity.Waiting, Activity.Any);

            var statistic = uut.Evaluate(Times(WindowSpan, 2));

            Assert.AreEqual(Times(WindowSpan, 3), statistic[Activity.Away]);
            Assert.AreEqual(TimeSpan.Zero, statistic[Activity.Waiting]);
        }

        [Test]
        public void CountsWaitingIfIndividualPeriodsAreShorterThanThreshold()
        {
            var uut = Stream(Activity.Waiting, Activity.Waiting, Activity.Any, Activity.Waiting, Activity.Waiting);

            var statistic = uut.Evaluate(Times(WindowSpan, 3));

            Assert.AreEqual(Times(WindowSpan, 4), statistic[Activity.Waiting]);
        }

        private static ActivityStream Stream(params Activity[] activities)
        {
            var activityStream = new ActivityStream(WindowSpan);
            activityStream.AddAll(activities);
            return activityStream;
        }

        public static TimeSpan Times(TimeSpan span, int factor)
        {
            return TimeSpan.FromTicks(factor*span.Ticks);
        }
    }
}
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

using System.Collections.Generic;
using System.Linq;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Activities.SlidingWindow;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities.SlidingWindow
{
    [TestFixture]
    internal class FrequencyActivityMergeStrategyTest
    {
        [Test]
        public void ReturnsWaitingForEmptyWindow()
        {
            var uut = new FrequencyActivityMergeStrategy();

            var actual = uut.Merge(EmptyWindow());

            Assert.AreEqual(Activity.Waiting, actual);
        }

        private IList<Activity> EmptyWindow()
        {
            return Window( /* no activities */);
        }

        private IList<Activity> Window(params Activity[] activities)
        {
            return activities.ToList();
        }
    }
}
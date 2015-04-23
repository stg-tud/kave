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
using KaVE.FeedbackProcessor.Statistics;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class DeveloperStatisticsCalculatorTest : FeedbackDatabaseBasedTest
    {
        private DeveloperStatisticsCalculator _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new DeveloperStatisticsCalculator(TestFeedbackDatabase);
        }

        [Test]
        public void DeterminesDaysOfActivity()
        {
            const string sessionId = "sessionA";
            var developer = GivenDeveloperExists(new[] { sessionId });
            GivenEventExists(sessionId, triggeredAt: new DateTime(2015, 04, 01, 23, 01, 42));
            GivenEventExists(sessionId, triggeredAt: new DateTime(2015, 04, 01, 23, 23, 42));
            GivenEventExists(sessionId, triggeredAt: new DateTime(2015, 03, 31, 15, 12, 59));

            var actuals = _uut.GetActiveDays(developer);

            var expecteds = new[] { new DateTime(2015, 4, 1), new DateTime(2015, 3, 31) };
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }
    }
}
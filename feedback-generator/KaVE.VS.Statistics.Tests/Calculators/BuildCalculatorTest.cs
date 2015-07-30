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

using System.Collections.Generic;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.VS.Statistics.Calculators;
using KaVE.VS.Statistics.Statistics;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Calculators
{
    [TestFixture]
    internal class BuildCalculatorTest : CalculatorTest
    {
        public BuildCalculatorTest() : base(typeof (BuildCalculator), typeof (BuildStatistic), new BuildEvent()) {}

        protected override bool IsNewStatistic(IStatistic statistic)
        {
            var buildStatistic = statistic as BuildStatistic;
            if (buildStatistic == null)
            {
                return false;
            }
            return buildStatistic.FailedBuilds == 0 &&
                   buildStatistic.SuccessfulBuilds == 0 &&
                   buildStatistic.TotalBuilds == 0;
        }

        private static readonly BuildTarget SuccessfulBuildTarget = new BuildTarget {Successful = true};
        private static readonly BuildTarget FailedBuildTarget = new BuildTarget {Successful = false};
        private static readonly List<BuildTarget> SuccessfulTargetList = new List<BuildTarget> {SuccessfulBuildTarget};
        private static readonly List<BuildTarget> FailedTargetList = new List<BuildTarget> {FailedBuildTarget};

        internal static readonly object[] BuildEvents =
        {
            new object[]
            {
                new[]
                {
                    new BuildEvent {Targets = FailedTargetList},
                    new BuildEvent {Targets = SuccessfulTargetList},
                    new BuildEvent {Targets = FailedTargetList},
                    new BuildEvent {Targets = FailedTargetList}
                },
                1,
                3
            }
        };

        [Test, TestCaseSource("BuildEvents")]
        public void CalculatingFromEventsTest(BuildEvent[] buildEvents,
            int expectedSuccessfulBuilds,
            int expectedFailedBuilds)
        {
            var expectedTotalBuilds = expectedSuccessfulBuilds + expectedFailedBuilds;
            var actualStatistic = (BuildStatistic) ListingMock.Object.GetStatistic(StatisticType);

            Publish(buildEvents);

            Assert.AreEqual(expectedSuccessfulBuilds, actualStatistic.SuccessfulBuilds);
            Assert.AreEqual(expectedFailedBuilds, actualStatistic.FailedBuilds);
            Assert.AreEqual(expectedTotalBuilds, actualStatistic.TotalBuilds);
        }

        [Test]
        public void EventCallWithWrongEventTypeExceptionHandlingTest()
        {
            Publish(new TestIDEEvent());

            ListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }
    }
}
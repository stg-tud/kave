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

        [Test]
        public void ComputesLowerBoundToNumberOfParticipants_Overlapping()
        {
            // developer 1
            const string dev1Session = "dev1";
            GivenDeveloperExists(new[] { dev1Session });
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 04, 02));
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 04, 01));
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 03, 31));
            // developer 2
            const string dev2Session = "dev2";
            GivenDeveloperExists(new[] { dev2Session });
            GivenEventExists(dev2Session, triggeredAt: new DateTime(2015, 04, 03));
            GivenEventExists(dev2Session, triggeredAt: new DateTime(2015, 04, 02));

            var actual = _uut.GetLowerBoundToNumberOfParticipants();

            Assert.AreEqual(2, actual);
        }

        [Test]
        public void ComputesLowerBoundToNumberOfParticipants_Separate()
        {
            // developer 1
            const string dev1Session = "dev1";
            GivenDeveloperExists(new[] { dev1Session });
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 04, 01));
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 03, 31));
            // developer 2
            const string dev2Session = "dev2";
            GivenDeveloperExists(new[] { dev2Session });
            GivenEventExists(dev2Session, triggeredAt: new DateTime(2015, 04, 03));
            GivenEventExists(dev2Session, triggeredAt: new DateTime(2015, 04, 02));

            var actual = _uut.GetLowerBoundToNumberOfParticipants();

            Assert.AreEqual(1, actual);
        }

        [Test]
        public void ComputesLowerBoundToNumberOfParticipants_Multiple1()
        {
            // developer 1
            const string dev1Session = "dev1";
            GivenDeveloperExists(new[] { dev1Session });
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 04, 02));
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 04, 01));
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 03, 31));
            // developer 2
            const string dev2Session = "dev2";
            GivenDeveloperExists(new[] { dev2Session });
            GivenEventExists(dev2Session, triggeredAt: new DateTime(2015, 04, 03));
            GivenEventExists(dev2Session, triggeredAt: new DateTime(2015, 04, 02));
            // developer 3
            const string dev3Session = "dev3";
            GivenDeveloperExists(new[] { dev3Session });
            GivenEventExists(dev3Session, triggeredAt: new DateTime(2015, 04, 01));
            GivenEventExists(dev3Session, triggeredAt: new DateTime(2015, 03, 31));
            // developer 4
            const string dev4Session = "dev4";
            GivenDeveloperExists(new[] { dev4Session });
            GivenEventExists(dev4Session, triggeredAt: new DateTime(2015, 04, 04));
            GivenEventExists(dev4Session, triggeredAt: new DateTime(2015, 04, 03));

            var actual = _uut.GetLowerBoundToNumberOfParticipants();

            Assert.AreEqual(2, actual);
        }

        [Test]
        public void ComputesLowerBoundToNumberOfParticipants_Multiple2()
        {
            // developer 1
            const string dev1Session = "dev1";
            GivenDeveloperExists(new[] { dev1Session });
            GivenEventExists(dev1Session, triggeredAt: new DateTime(2015, 04, 01));
            // developer 2
            const string dev2Session = "dev2";
            GivenDeveloperExists(new[] { dev2Session });
            GivenEventExists(dev2Session, triggeredAt: new DateTime(2015, 04, 02));
            // developer 3
            const string dev3Session = "dev3";
            GivenDeveloperExists(new[] { dev3Session });
            GivenEventExists(dev3Session, triggeredAt: new DateTime(2015, 04, 02));

            var actual = _uut.GetLowerBoundToNumberOfParticipants();

            Assert.AreEqual(2, actual);
        }

        [Test]
        public void ExcludesAnonymouseDevelopersFromLowerBound()
        {
            // developer 1
            var dev1 = GivenDeveloperExists(/* anonymous */);
            GivenEventExists(dev1.Id.ToString(), triggeredAt: new DateTime(2015, 04, 01));

            var actual = _uut.GetLowerBoundToNumberOfParticipants();

            Assert.AreEqual(0, actual);
        }

        [Test]
        public void UpperBoundToNumberOfParticipantsIsNumberOfDevelopers()
        {
            GivenDeveloperExists("dev1");
            GivenDeveloperExists("dev2");
            GivenDeveloperExists("dev3");

            var actual = _uut.GetUpperBoundToNumberOfParticipants();

            Assert.AreEqual(3, actual);
        }

        [Test]
        public void ComputesNumberOfSessionsAssignedToMultipleDevelopers()
        {
            GivenDeveloperExists("sessionA", "sessionB");
            GivenDeveloperExists("sessionB", "sessionC");
            GivenDeveloperExists("sessionD", "sessionE");
            GivenDeveloperExists("sessionE");

            var actual = _uut.GetNumberOfSessionsAssignedToMultipleDevelopers();

            Assert.AreEqual(2, actual);
        }

        [Test]
        public void ComputesNumberOfSessions()
        {
            GivenDeveloperExists("sessionA", "sessionB");
            GivenDeveloperExists("sessionA");
            GivenDeveloperExists("sessionC");

            var actual = _uut.GetNumberOfSessions();

            Assert.AreEqual(3, actual);
        }
    }
}
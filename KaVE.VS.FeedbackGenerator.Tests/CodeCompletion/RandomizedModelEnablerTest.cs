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
using System.Collections;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.CodeCompletion;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.CodeCompletion
{
    internal class RandomizedModelEnablerTest
    {
        private RandomizedModelEnabler _sut;
        private UserProfileSettings _userProfileSettings;

        [SetUp]
        public void SetUp()
        {
            var userProfileId = Guid.NewGuid().ToString();
            _userProfileSettings = new UserProfileSettings {ProfileId = userProfileId};
            var ss = new Mock<ISettingsStore>();
            ss.Setup(u => u.GetSettings<UserProfileSettings>()).Returns(_userProfileSettings);
            _sut = new RandomizedModelEnabler(ss.Object);
        }

        [Test]
        public void CachingVersion()
        {
            var typeName = "System.String";

            Assert.AreEqual(
                _sut.IsEnabled(typeName),
                RandomizedModelEnabler.IsEnabledForUserAndType(
                    _userProfileSettings.ProfileId,
                    typeName,
                    RandomizedModelEnabler.AvailabilityChance));
        }

        private IEnumerable TestCases()
        {
            for (int i = 0; i < 100; i++)
            {
                yield return new TestCaseData(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }
        }

        [Test, TestCaseSource("TestCases")]
        public void AllAvailableWith100PercentProbability(string profileId, string typeName)
        {
            Assert.IsTrue(RandomizedModelEnabler.IsEnabledForUserAndType(profileId, typeName, 100));
        }

        [Test, TestCaseSource("TestCases")]
        public void NoneAvailableWith0PercentProbability(string profileId, string typeName)
        {
            Assert.IsFalse(RandomizedModelEnabler.IsEnabledForUserAndType(profileId, typeName, 0));
        }

        [Test]
        public void ArbitraryTypesAreRandomlyActivated()
        {
            var count = CountEnabled(1000, "SomeType, P");
            Assert.True(count > 450);
            Assert.True(count < 550);
        }

        [Test]
        public void StringBuilderIsAlwaysActive()
        {
            Assert.AreEqual(100, CountEnabled(100, "System.Text.StringBuilder, mscorlib, 4.0.0.0"));
        }

        private static int CountEnabled(int reps, string type)
        {
            var numEnabled = 0;
            for (var i = 0; i < reps; i++)
            {
                var profileId = "pid" + i;

                if (RandomizedModelEnabler.IsEnabledForUserAndType(profileId, type, 50))
                {
                    numEnabled++;
                }
            }
            return numEnabled;
        }

        [Test]
        public void ResultIsDeterministic()
        {
            var profileId = Guid.NewGuid().ToString();
            var typeName = Guid.NewGuid().ToString();
            var firstResult = RandomizedModelEnabler.IsEnabledForUserAndType(profileId, typeName, 50);

            for (int i = 0; i < 100; i++)
            {
                var repeatedResult = RandomizedModelEnabler.IsEnabledForUserAndType(profileId, typeName, 50);
                Assert.AreEqual(firstResult, repeatedResult);
            }
        }
    }
}
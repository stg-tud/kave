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

using System.Net.Mail;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Generators;
using KaVE.VS.FeedbackGenerator.Settings;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators
{
    internal class UserProfileEventGeneratorTest : EventGeneratorTestBase
    {
        private const string TestUserName = "Some Name";
        private static readonly MailAddress TestMailAddress = new MailAddress("test@mail.com");
        private const int TestNumber = 42;
        /*    private const Category TestCategory = Category.Category2;
        private const Valuation TestValuation = Valuation.Valuation2; */
        private const string TestFeedback = "this tool is awesome!";

        private UserProfileSettings _testUserSettings;

        private Mock<ISettingsStore> _testSettingsStore;

        private UserProfileEventGenerator _uut;

        [SetUp]
        public void Setup()
        {
            _testUserSettings = new UserProfileSettings
            {
                //      Category = TestCategory,
                //Email = TestMailAddress.ToString(),
                Feedback = TestFeedback,
                //ExperienceYears = TestNumber,
                IsProvidingProfile = true,
                //Name = TestUserName
                //     Valuation = TestValuation
            };

            _testSettingsStore = new Mock<ISettingsStore>();
            _testSettingsStore.Setup(settingsStore => settingsStore.GetSettings<UserProfileSettings>())
                              .Returns(_testUserSettings);

            _uut = new UserProfileEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, _testSettingsStore.Object);
        }

        [Test]
        public void ShouldNotPublishAnyEvents()
        {
            _testUserSettings.IsProvidingProfile = true;

            //_uut.CreateExportEvent();

            AssertNoEvent();
        }

        [Test]
        public void ShouldSetUserProfileIfEnabled()
        {
            _testUserSettings.IsProvidingProfile = true;

            var actualEvent = _uut.CreateExportEvent();

          //  Assert.AreEqual(TestUserName, actualEvent.Name);
          //  Assert.AreEqual(TestMailAddress, actualEvent.Email);
            //   Assert.AreEqual(TestCategory, actualEvent.Category);
            //        Assert.AreEqual(TestNumber, actualEvent.Number);
            //  Assert.AreEqual(TestValuation, actualEvent.Valuation);
            Assert.AreEqual(TestFeedback, actualEvent.Feedback);
        }

        [Test]
        public void ShouldNotSetUserProfileIfDisabled()
        {
            _testUserSettings.IsProvidingProfile = false;

            var actualEvent = _uut.CreateExportEvent();

         //   Assert.AreNotEqual(TestUserName, actualEvent.Name);
          //  Assert.AreNotEqual(TestMailAddress, actualEvent.Email);
            //  Assert.AreNotEqual(TestCategory, actualEvent.Category);
            //   Assert.AreNotEqual(TestNumber, actualEvent.Number);
            //  Assert.AreNotEqual(TestValuation, actualEvent.Valuation);
        }

        [Test]
        public void ShouldAcceptEmptyFields()
        {
            _testUserSettings.IsProvidingProfile = true;
            //   _testUserSettings.Category = Category.Unknown;
           // _testUserSettings.Email = "";
            _testUserSettings.Feedback = "";
           // _testUserSettings.ExperienceYears = 0;
          //  _testUserSettings.Name = "";
            //  _testUserSettings.Valuation = Valuation.Unknown;

            _uut.CreateExportEvent();
        }
    }
}
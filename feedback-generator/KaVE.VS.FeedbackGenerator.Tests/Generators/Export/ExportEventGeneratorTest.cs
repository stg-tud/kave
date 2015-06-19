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
using KaVE.Commons.Model.Events.Export;
using KaVE.VS.FeedbackGenerator.Utils;
using KaVE.VS.FeedbackGenerator.Generators.Export;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation.UserSetting;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Export
{
    internal class ExportEventGeneratorTest : EventGeneratorTestBase
    {
        private const string TestUserName = "Some Name";
        private static readonly MailAddress TestMailAddress = new MailAddress("test@mail.com");
        private const int TestNumber = 42;
        private const Category TestCategory = Category.Category2;
        private const Valuation TestValuation = Valuation.Valuation2;
        private const string TestFeedback = "this tool is awesome!";

        private UserSettings _testUserSettings;

        private Mock<ISettingsStore> _testSettingsStore;

        private ExportEventGenerator _uut;

        [SetUp]
        public void Setup()
        {
            _testUserSettings = new UserSettings
            {
                Category = TestCategory,
                Mail = TestMailAddress.ToString(),
                Feedback = TestFeedback,
                NumberField = TestNumber.ToString(),
                ProvideUserInformation = true,
                Username = TestUserName,
                Valuation = TestValuation
            };

            _testSettingsStore = new Mock<ISettingsStore>();
            _testSettingsStore.Setup(settingsStore => settingsStore.GetSettings<UserSettings>())
                              .Returns(_testUserSettings);

            _uut = new ExportEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, _testSettingsStore.Object);
        }

        [Test]
        public void ShouldNotPublishAnyEvents()
        {
            _testUserSettings.ProvideUserInformation = true;

            _uut.CreateExportEvent();

            AssertNoEvent();
        }

        [Test]
        public void ShouldSetUserProfileIfEnabled()
        {
            _testUserSettings.ProvideUserInformation = true;
            
            var actualEvent = _uut.CreateExportEvent();

            Assert.AreEqual(TestUserName, actualEvent.UserName);
            Assert.AreEqual(TestMailAddress, actualEvent.Mail);
            Assert.AreEqual(TestCategory, actualEvent.Category);
            Assert.AreEqual(TestNumber, actualEvent.Number);
            Assert.AreEqual(TestValuation, actualEvent.Valuation);
            Assert.AreEqual(TestFeedback, actualEvent.Feedback);
        }

        [Test]
        public void ShouldNotSetUserProfileIfDisabled()
        {
            _testUserSettings.ProvideUserInformation = false;

            var actualEvent = _uut.CreateExportEvent();

            Assert.AreNotEqual(TestUserName, actualEvent.UserName);
            Assert.AreNotEqual(TestMailAddress, actualEvent.Mail);
            Assert.AreNotEqual(TestCategory, actualEvent.Category);
            Assert.AreNotEqual(TestNumber, actualEvent.Number);
            Assert.AreNotEqual(TestValuation, actualEvent.Valuation);
        }

        [Test]
        public void ShouldAcceptEmptyFields()
        {
            _testUserSettings.ProvideUserInformation = true;
            _testUserSettings.Category = Category.Unknown;
            _testUserSettings.Mail = "";
            _testUserSettings.Feedback = "";
            _testUserSettings.NumberField = "";
            _testUserSettings.Username = "";
            _testUserSettings.Valuation = Valuation.Unknown;

            _uut.CreateExportEvent();
        }
    }
}
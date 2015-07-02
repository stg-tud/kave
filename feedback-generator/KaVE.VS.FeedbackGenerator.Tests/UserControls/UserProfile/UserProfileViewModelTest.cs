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
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Tests.Interactivity;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfile
{
    public class UserProfileViewModelTest
    {
        private Mock<ISettingsStore> _mockSettingStore;

        private UserProfileSettings _userSettings;

        private UserProfileContext _uut;
        private InteractionRequestTestHelper<Notification> _notificationHelper;

        [SetUp]
        public void SetUp()
        {
            _userSettings = new UserProfileSettings();
            _mockSettingStore = new Mock<ISettingsStore>();
            _mockSettingStore.Setup(store => store.GetSettings<ExportSettings>()).Returns(new ExportSettings());

            //_uut = new UserProfileContext(_mockSettingStore.Object) {UserProfileSettings = _userSettings};

            //  _notificationHelper = _uut.ErrorNotificationRequest.NewTestHelper();
        }

        [TestCase(true), TestCase(false)]
        public void ShouldGetUserSettingsProvideUserInformation(bool expected)
        {
            _userSettings.ProvideUserInformation = expected;

            var actual = _uut.ProvideUserInformation;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldSetUserSettingsProvideUserInformationAfterSetUserSettingsIsCalled(bool expected)
        {
            _uut.ProvideUserInformation = expected;
            _uut.SetUserSettings();

            _mockSettingStore.Verify(ss => ss.SetSettings(It.IsAny<UserProfileSettings>()));
            Assert.AreEqual(expected, _userSettings.ProvideUserInformation);
        }

        [Test]
        public void ShouldGetUserSettingsUserName()
        {
            var expected = "Max Mustermann";
            _userSettings.Name = expected;

            var actual = _uut.Username;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetUserSettingsUsernameAfterSetUserSettingsIsCalled()
        {
            var expected = "Max Mustermann";
            _uut.Username = expected;
            _uut.SetUserSettings();

            _mockSettingStore.Verify(ss => ss.SetSettings(It.IsAny<UserProfileSettings>()));
            Assert.AreEqual(expected, _userSettings.Name);
        }

        [Test]
        public void ShouldGetUserSettingsMail()
        {
            var expected = "test@mail.com";
            _userSettings.Email = expected;

            var actual = _uut.Mail;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetUserSettingsEmailAfterSetUserSettingsIsCalled()
        {
            var expected = "test@mail.com";
            _uut.Mail = expected;
            _uut.SetUserSettings();

            _mockSettingStore.Verify(ss => ss.SetSettings(It.IsAny<UserProfileSettings>()));
            Assert.AreEqual(expected, _userSettings.Email);
        }

        [Test]
        public void ShouldGetUserSettingsNumberField()
        {
            var expected = "42";
            _userSettings.ExperienceYears = 42;

            var actual = _uut.NumberText;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetUserSettingsNumberFieldAfterSetUserSettingsIsCalled()
        {
            var expected = "42";
            _uut.NumberText = expected;
            _uut.SetUserSettings();

            _mockSettingStore.Verify(ss => ss.SetSettings(It.IsAny<UserProfileSettings>()));
            //Assert.AreEqual(expected, _userSettings.NumberField);
        }

        [Test]
        public void ShouldGetUserSettingsCategory()
        {
            /*var expected = Category.Category1;
            _userSettings.Category= expected;

            var actual = _uut.Category;

            Assert.AreEqual(expected, actual);*/
        }

        [Test]
        public void ShouldSetUserSettingsCategoryAfterSetUserSettingsIsCalled()
        {
            /*     var expected = Category.Category1;
            _uut.Category = expected;
            _uut.SetUserSettings();

            _mockSettingStore.Verify(ss => ss.SetSettings(It.IsAny<UserSettings>()));
            Assert.AreEqual(expected, _userSettings.Category);*/
        }

        [Test]
        public void ShouldGetUserSettingsValuation()
        {
            /*         var expected = Valuation.Valuation1;
            _userSettings.Valuation = expected;

            var actual = _uut.Valuation;

            Assert.AreEqual(expected, actual);*/
        }

        [Test]
        public void ShouldSetUserSettingsValuationAfterSetUserSettingsIsCalled()
        {
            /*          var expected = Valuation.Valuation1;
            _uut.Valuation = expected;
            _uut.SetUserSettings();

            _mockSettingStore.Verify(ss => ss.SetSettings(It.IsAny<UserSettings>()));
            Assert.AreEqual(expected, _userSettings.Valuation);*/
        }

        [Test]
        public void InvalidMailRaisesErrorNotification()
        {
            var mailValidationResult = _uut.ValidateEmail("InvalidMail");
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
            Assert.IsFalse(mailValidationResult);
        }

        [Test]
        public void ValidMailRaisesNoErrorNotification()
        {
            var mailValidationResult = _uut.ValidateEmail("test@mail.com");
            Assert.IsFalse(_notificationHelper.IsRequestRaised);
            Assert.IsTrue(mailValidationResult);
        }

        [Test]
        public void InvalidMailErrorHasCorrectMessage()
        {
            _uut.ValidateEmail("InvalidMail");

            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = Properties.UserProfileMessages.EmailValidationErrorTitle,
                Message = Properties.UserProfileMessages.EmailValidationErrorMessage
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotProvideUserInformationWhenDatevUser()
        {
            var exportSettings = new ExportSettings {IsDatev = true};
            _mockSettingStore.Setup(store => store.GetSettings<ExportSettings>()).Returns(exportSettings);

            //  _uut = new UserProfileContext(_mockSettingStore.Object) {UserProfileSettings = _userSettings};

            _mockSettingStore.Verify(ss => ss.UpdateSettings(It.IsAny<Action<UserProfileSettings>>()));

            Assert.IsFalse(_uut.ProvideUserInformation);
        }
    }
}
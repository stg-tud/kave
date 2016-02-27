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
using JetBrains.Application.DataContext;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Menu;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Menu
{
    public class UploadWizardActionTest
    {
        private Mock<ISettingsStore> _mockSettingsStore;
        private Mock<IUploadWizardWindowCreator> _mockUploadWizardWindowCreator;
        private Mock<ILogManager> _mockLogManager;

        private UploadWizardAction _uut;

        [SetUp]
        public void Setup()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockUploadWizardWindowCreator = new Mock<IUploadWizardWindowCreator>();
            _mockLogManager = new Mock<ILogManager>();

            Registry.RegisterComponent(_mockSettingsStore.Object);
            Registry.RegisterComponent(_mockUploadWizardWindowCreator.Object);
            Registry.RegisterComponent(_mockLogManager.Object);

            _uut = new UploadWizardAction();
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldOpenUserProfileReminderOnFirstUpload()
        {
            var userProfileSettings = new UserProfileSettings
            {
                HasBeenAskedToFillProfile = false
            };
            var exportSettings = new ExportSettings
            {
                IsDatev = false
            };
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>()).Returns(exportSettings);
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(userProfileSettings);

            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Once);
        }

        [Test]
        public void ShouldNotOpenUserProfileReminderWhenDatevUser()
        {
            var exportSettings = new ExportSettings
            {
                IsDatev = true
            };
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>()).Returns(exportSettings);
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(new UserProfileSettings());

            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Never);
        }

        [Test]
        public void ShouldNotOpenUserProfileReminderWhenAskedBefore()
        {
            var userProfileSettings = new UserProfileSettings
            {
                HasBeenAskedToFillProfile = true
            };

            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>())
                              .Returns(new ExportSettings());
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(userProfileSettings);

            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Never);
        }

        [Test]
        public void ShouldNotOpenUserProfileReminderWhenUserAlreadyProvidesUserProfile()
        {
            var userProfileSettings = new UserProfileSettings();

            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>())
                              .Returns(new ExportSettings());
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(userProfileSettings);

            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Never);
        }

        [Test]
        public void ShouldShowUploadWizardWhenContentToExportAndUserProfileHasBeenAsked()
        {
            var userProfileSettings = new UserProfileSettings
            {
                HasBeenAskedToFillProfile = true
            };

            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>())
                              .Returns(new ExportSettings());
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(userProfileSettings);

            var logs = new List<ILog>
            {
                new Mock<ILog>().Object,
                new Mock<ILog>().Object
            };
            _mockLogManager.Setup(logManager => logManager.Logs).Returns(logs);

            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Never);
            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUploadWizardControl(),
                Times.Once);
        }

        [Test]
        public void ShouldShowNothingToExportDialogWhenNoContentToExport()
        {
            var userProfileSettings = new UserProfileSettings
            {
                HasBeenAskedToFillProfile = true
            };

            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>())
                              .Returns(new ExportSettings());
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(userProfileSettings);

            _mockLogManager.Setup(logManager => logManager.Logs).Returns(new List<ILog>());

            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Never);
            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUploadWizardControl(),
                Times.Never);
            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenNothingToExportDialog(),
                Times.Once);
        }
    }
}
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
        private UserProfileSettings _userProfileSettings;
        private ExportSettings _exportSettings;

        [SetUp]
        public void Setup()
        {
            _userProfileSettings = new UserProfileSettings();

            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockUploadWizardWindowCreator = new Mock<IUploadWizardWindowCreator>();
            _mockLogManager = new Mock<ILogManager>();

            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(_userProfileSettings);
            _exportSettings = new ExportSettings();
            _mockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>())
                              .Returns(_exportSettings);


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
            _userProfileSettings.HasBeenAskedToFillProfile = false;

            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Once);
        }

        [Test]
        public void ShouldNotOpenUserProfileDialogWhenAskedBefore()
        {
            _userProfileSettings.HasBeenAskedToFillProfile = true;


            _uut.Execute(new Mock<IDataContext>().Object, null);

            _mockUploadWizardWindowCreator.Verify(
                uploadWizardWindowCreator => uploadWizardWindowCreator.OpenUserProfileReminderDialog(),
                Times.Never);
        }

        [Test]
        public void ShouldShowUploadDialogWhenContentToExportAndUserProfileHasBeenAsked()
        {
            _userProfileSettings.HasBeenAskedToFillProfile = true;

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
            _userProfileSettings.HasBeenAskedToFillProfile = true;

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
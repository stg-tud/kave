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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using JetBrains;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.TestUtils.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.Tests.Interactivity;
using KaVE.VS.FeedbackGenerator.UserControls.UploadWizard;
using KaVE.VS.FeedbackGenerator.Utils.Export;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UploadWizard
{
    internal class UploadWizardContextTest
    {
        private const string TestUploadUrl = "http://foo.bar/";

        private Mock<ILogManager> _mockLogFileManager;
        private List<Mock<ILog>> _mockLogs;
        private UploadWizardContext _uut;
        private Mock<IExporter> _mockExporter;
        private Mock<ISettingsStore> _mockSettingStore;
        private InteractionRequestTestHelper<Notification> _notificationHelper;
        private InteractionRequestTestHelper<LinkNotification> _linkNotificationHelper;
        private TestDateUtils _testDateUtils;
        private Mock<IIoUtils> _mockIoUtils;
        private Mock<ILogger> _mockLogger;
        private ExportSettings _exportSettings;
        private UserProfileSettings _userSettings;
        private AnonymizationSettings _anonymizationSettings;

        [SetUp]
        public void SetUp()
        {
            _mockIoUtils = new Mock<IIoUtils>();
            Registry.RegisterComponent(_mockIoUtils.Object);

            _mockLogger = new Mock<ILogger>();

            _mockExporter = new Mock<IExporter>();

            _mockLogs = new List<Mock<ILog>> {new Mock<ILog>(), new Mock<ILog>(), new Mock<ILog>()};

            _mockLogFileManager = new Mock<ILogManager>();
            _mockLogFileManager.Setup(mgr => mgr.Logs).Returns(_mockLogs.Select(m => m.Object));

            _mockSettingStore = new Mock<ISettingsStore>();
            _mockSettingStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(new UploadSettings());
            _exportSettings = new ExportSettings {UploadUrl = TestUploadUrl};
            _userSettings = new UserProfileSettings();
            _anonymizationSettings = new AnonymizationSettings();
            _mockSettingStore.Setup(store => store.GetSettings<ExportSettings>()).Returns(_exportSettings);
            _mockSettingStore.Setup(store => store.GetSettings<UserProfileSettings>()).Returns(_userSettings);
            _mockSettingStore.Setup(store => store.GetSettings<AnonymizationSettings>()).Returns(_anonymizationSettings);
            _mockSettingStore.Setup(store => store.UpdateSettings(It.IsAny<Action<ExportSettings>>()))
                             .Callback<Action<ExportSettings>>(update => update(_exportSettings));

            _testDateUtils = new TestDateUtils();
            _uut = new UploadWizardContext(
                _mockExporter.Object,
                _mockLogFileManager.Object,
                _mockSettingStore.Object,
                _testDateUtils,
                _mockLogger.Object);

            _notificationHelper = _uut.ErrorNotificationRequest.NewTestHelper();
            _linkNotificationHelper = _uut.SuccessNotificationRequest.NewTestHelper();
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }
        
        // TODO @Seb: re-enable
        [Test, Ignore]
        public void ShouldGetUserSettingsFeedback()
        {
            var expected = "Good Feedback";
            _userSettings.Comment = expected;

            var actual = _uut.Comment;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetUserSettingsFeedbackAfterSetSettingsIsCalled()
        {
            var expected = "Good Feedback";
            _userSettings.Comment = expected;

            _uut.SetSettings();

            _mockSettingStore.Verify(ss => ss.UpdateSettings(It.IsAny<Action<UserProfileSettings>>()));
            Assert.AreEqual(expected, _userSettings.Comment);
        }

        [Test]
        public void ShouldSetExportBusyMessage()
        {
            _uut.Export(UploadWizardControl.ExportType.ZipFile);

            Assert.IsTrue(_uut.BusyMessage.StartsWith(Properties.UploadWizard.Export_BusyMessage));
        }

        [Test]
        public void ShouldPropagateExportStatus()
        {
            _mockExporter.Setup(e => e.Export(It.IsAny<DateTime>(), It.IsAny<IPublisher>()))
                         .Callback(
                             () =>
                             {
                                 _mockExporter.Raise(e => e.StatusChanged += null, "13%");
                                 _mockExporter.Raise(e => e.StatusChanged += null, "42%");
                                 _mockExporter.Raise(e => e.StatusChanged += null, "finishing");
                             });

            var expected = new List<string>
            {
                Properties.UploadWizard.Export_BusyMessage,
                Properties.UploadWizard.Export_BusyMessage + " (13%)",
                Properties.UploadWizard.Export_BusyMessage + " (42%)",
                Properties.UploadWizard.Export_BusyMessage + " (finishing)"
            };

            var actual = new List<string>();

            _uut.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BusyMessage")
                {
                    actual.Add(_uut.BusyMessage);
                }
            };

            WhenExportIsExecuted();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SelectingZipExportInvokesZipExport()
        {
            WhenExportIsExecuted(UploadWizardControl.ExportType.ZipFile);

            _mockExporter.Verify(
                exporter =>
                    exporter.Export(It.IsAny<DateTime>(), It.IsAny<FilePublisher>()));
        }

        [Test]
        public void SelectingHttpExportInvokesHttpExport()
        {
            WhenExportIsExecuted(UploadWizardControl.ExportType.HttpUpload);

            _mockExporter.Verify(
                exporter =>
                    exporter.Export(It.IsAny<DateTime>(), It.IsAny<HttpPublisher>()));
        }

        [Test]
        public void ShouldUseUploadUrlFromSettingsForUpload()
        {
            HttpPublisher httpPublisher = null;
            _mockExporter.Setup(
                exporter =>
                    exporter.Export(It.IsAny<DateTime>(), It.IsAny<HttpPublisher>()))
                         .Callback<DateTime, IPublisher>(
                             (exportTime, publisher) => httpPublisher = (HttpPublisher) publisher);

            WhenExportIsExecuted(UploadWizardControl.ExportType.HttpUpload);
            try
            {
                // There's currently no clean way to find out what Url is passed to the publisher. Therefore, we
                // invoke the publisher and rely on that it calls IIoUtils.TransferByHttp and then fails because the
                // utils return null.
                httpPublisher.Publish(new MemoryStream());
            }
            catch (NullReferenceException) {}

            _mockIoUtils.Verify(
                ioUtils => ioUtils.TransferByHttp(It.IsAny<HttpContent>(), new Uri(TestUploadUrl)));
        }

        [Test]
        public void SuccessfulDirectUploadCreatesNotification()
        {
            WhenExportIsExecuted();

            Assert.IsTrue(_notificationHelper.IsRequestRaised);
        }

        [Test]
        public void SuccessfulZipExportCreatesNotification()
        {
            WhenExportIsExecuted(UploadWizardControl.ExportType.ZipFile);

            Assert.IsTrue(_linkNotificationHelper.IsRequestRaised);
        }

        [Test]
        public void SuccessfulZipExportNotificationHasCorrectMessage()
        {
            _mockExporter.Setup(
                e => e.Export(_testDateUtils.Now, It.IsAny<IPublisher>())).Returns(23);

            WhenExportIsExecuted(UploadWizardControl.ExportType.ZipFile);

            var actual = _linkNotificationHelper.Context;
            var expected = new LinkNotification
            {
                Caption = UploadWizardMessages.Title,
                Message = Properties.UploadWizard.ExportSuccess.FormatEx(23),
                LinkDescription = Properties.UploadWizard.ExportSuccessLinkDescription,
                Link = TestUploadUrl
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SuccessfulDirectUploadNotificationHasCorrectMessage()
        {
            _mockExporter.Setup(
                e => e.Export(_testDateUtils.Now, It.IsAny<IPublisher>())).Returns(42);

            WhenExportIsExecuted();

            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = UploadWizardMessages.Title,
                Message = Properties.UploadWizard.ExportSuccess.FormatEx(42)
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FailingExportCreatesNotification()
        {
            _mockExporter.Setup(
                e => e.Export(It.IsAny<DateTime>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));

            WhenExportIsExecuted();

            Assert.IsTrue(_notificationHelper.IsRequestRaised);
        }

        [Test]
        public void FailingExportNotificationHasCorrectMessage()
        {
            _mockExporter.Setup(
                e => e.Export(It.IsAny<DateTime>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));

            WhenExportIsExecuted();

            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = UploadWizardMessages.Title,
                Message = Properties.UploadWizard.ExportFail + ":\nTEST"
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FailingExportLogsError()
        {
            var exception = new AssertException("TEST");
            _mockExporter.Setup(
                e => e.Export(It.IsAny<DateTime>(), It.IsAny<IPublisher>())).Throws(exception);

            WhenExportIsExecuted();

            _mockLogger.Verify(l => l.Error(exception, "export failed"));
        }

        [Test]
        public void SuccessfulExportUpdatesLastUploadDate()
        {
            _testDateUtils.Now = DateTime.Now;
            UploadSettings uploadSettings = null;
            _mockSettingStore.Setup(store => store.SetSettings(It.IsAny<UploadSettings>()))
                             .Callback<UploadSettings>(settings => uploadSettings = settings);

            WhenExportIsExecuted();

            Assert.AreEqual(_testDateUtils.Now, uploadSettings.LastUploadDate);
        }

        [Test]
        public void PassesExportTimeToExporter()
        {
            _testDateUtils.Now = DateTime.Now;
            var invoked = false;
            _mockExporter.Setup(
                e => e.Export(_testDateUtils.Now, It.IsAny<IPublisher>()))
                         .Callback<DateTime, IPublisher>((export, p) => invoked = true);

            WhenExportIsExecuted();

            Assert.IsTrue(invoked);
        }

        [Test]
        public void DeletesEventsBeforeExportTime()
        {
            _testDateUtils.Now = DateTime.Now;

            WhenExportIsExecuted();

            _mockLogFileManager.Verify(lm => lm.DeleteLogsOlderThan(_testDateUtils.Now));
        }

        private void WhenExportIsExecuted()
        {
            // ReSharper disable once IntroduceOptionalParameters.Local
            WhenExportIsExecuted(UploadWizardControl.ExportType.HttpUpload);
        }

        private void WaitUntilViewModelIsIdle()
        {
            AsyncTestHelper.WaitForCondition(() => !_uut.IsBusy);
        }

        private void WhenExportIsExecuted(UploadWizardControl.ExportType exportType)
        {
            _uut.Export(exportType);
            WaitUntilViewModelIsIdle();
        }
    }
}
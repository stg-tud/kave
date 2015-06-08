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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using JetBrains;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO;
using KaVE.VsFeedbackGenerator.Export;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Tests.Interactivity;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Export
{
    [TestFixture]
    internal class UploadWizardViewModelTest
    {
        private const string TestUploadUrl = "http://foo.bar/";

        private Mock<ILogManager> _mockLogFileManager;
        private List<Mock<ILog>> _mockLogs;
        private UploadWizardViewModel _uut;
        private Mock<IExporter> _mockExporter;
        private Mock<ISettingsStore> _mockSettingStore;
        private InteractionRequestTestHelper<Notification> _notificationHelper;
        private InteractionRequestTestHelper<LinkNotification> _linkNotificationHelper;
        private TestDateUtils _testDateUtils;
        private Mock<IIoUtils> _mockIoUtils;
        private Mock<ILogger> _mockLogger;
        private ExportSettings _exportSettings;

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
            _mockSettingStore.Setup(store => store.GetSettings<ExportSettings>()).Returns(_exportSettings);
            _mockSettingStore.Setup(store => store.UpdateSettings(It.IsAny<Action<ExportSettings>>()))
                             .Callback<Action<ExportSettings>>(update => update(_exportSettings));

            _testDateUtils = new TestDateUtils();
            _uut = new UploadWizardViewModel(
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

        [TestCase(true), TestCase(false)]
        public void ShouldGetExportSettingRemoveCodeNames(bool expected)
        {
            _exportSettings.RemoveCodeNames = expected;

            var actual = _uut.RemoveCodeNames;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldSetExportSettingsRemoveCodeNames(bool expected)
        {
            _uut.RemoveCodeNames = expected;

            _mockSettingStore.Verify(ss => ss.UpdateSettings(It.IsAny<Action<ExportSettings>>()));
            Assert.AreEqual(expected, _exportSettings.RemoveCodeNames);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldGetExportSettingRemoveDurations(bool expected)
        {
            _exportSettings.RemoveDurations = expected;

            var actual = _uut.RemoveDurations;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldSetExportSettingsRemoveDurations(bool expected)
        {
            _uut.RemoveDurations = expected;

            _mockSettingStore.Verify(ss => ss.UpdateSettings(It.IsAny<Action<ExportSettings>>()));
            Assert.AreEqual(expected, _exportSettings.RemoveDurations);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldGetExportSettingRemoveSessionIDs(bool expected)
        {
            _exportSettings.RemoveSessionIDs = expected;

            var actual = _uut.RemoveSessionIDs;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldSetExportSettingsRemoveSessionIDs(bool expected)
        {
            _uut.RemoveSessionIDs = expected;

            _mockSettingStore.Verify(ss => ss.UpdateSettings(It.IsAny<Action<ExportSettings>>()));
            Assert.AreEqual(expected, _exportSettings.RemoveSessionIDs);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldGetExportSettingRemoveStarTimes(bool expected)
        {
            _exportSettings.RemoveStartTimes = expected;

            var actual = _uut.RemoveStartTimes;

            Assert.AreEqual(expected, actual);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldSetExportSettingsRemoveStartTimes(bool expected)
        {
            _uut.RemoveStartTimes = expected;

            _mockSettingStore.Verify(ss => ss.UpdateSettings(It.IsAny<Action<ExportSettings>>()));
            Assert.AreEqual(expected, _exportSettings.RemoveStartTimes);
        }

        [Test]
        public void ShouldSetExportBusyMessage()
        {
            _uut.Export(UploadWizard.ExportType.ZipFile);

            Assert.IsTrue(_uut.BusyMessage.StartsWith(Properties.UploadWizard.Export_BusyMessage));
        }

        [Test]
        public void ShouldPropagateExportStatus()
        {
            _mockExporter.Setup(e => e.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<IPublisher>()))
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
                string.Format(
                    "{0} ({1})",
                    Properties.UploadWizard.Export_BusyMessage,
                    Properties.UploadWizard.FetchingEvents),
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
            WhenExportIsExecuted(UploadWizard.ExportType.ZipFile);

            _mockExporter.Verify(
                exporter =>
                    exporter.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<FilePublisher>()));
        }

        [Test]
        public void SelectingHttpExportInvokesHttpExport()
        {
            WhenExportIsExecuted(UploadWizard.ExportType.HttpUpload);

            _mockExporter.Verify(
                exporter =>
                    exporter.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<HttpPublisher>()));
        }

        [Test]
        public void ShouldUseUploadUrlFromSettingsForUpload()
        {
            HttpPublisher httpPublisher = null;
            _mockExporter.Setup(
                exporter =>
                    exporter.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<HttpPublisher>()))
                         .Callback<IEnumerable<IDEEvent>, IPublisher>(
                             (evts, publisher) => httpPublisher = (HttpPublisher) publisher);

            WhenExportIsExecuted(UploadWizard.ExportType.HttpUpload);
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
            WhenExportIsExecuted(UploadWizard.ExportType.ZipFile);

            Assert.IsTrue(_linkNotificationHelper.IsRequestRaised);
        }

        [Test]
        public void SuccessfulZipExportNotificationHasCorrectMessage()
        {
            WhenExportIsExecuted(UploadWizard.ExportType.ZipFile);

            var actual = _linkNotificationHelper.Context;
            // TODO @Sven: extend setup to include some events that are exported here
            // TODO @Seb: help sven with above task
            var expected = new LinkNotification
            {
                Caption = Properties.UploadWizard.window_title,
                Message = Properties.UploadWizard.ExportSuccess.FormatEx(0),
                LinkDescription = Properties.UploadWizard.ExportSuccessLinkDescription,
                Link = TestUploadUrl
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SuccessfulDirectUploadNotificationHasCorrectMessage()
        {
            WhenExportIsExecuted();

            var actual = _notificationHelper.Context;
            // TODO @Sven: extend setup to include some events that are exported here
            // TODO @Seb: help sven with above task
            var expected = new Notification
            {
                Caption = Properties.UploadWizard.window_title,
                Message = Properties.UploadWizard.ExportSuccess.FormatEx(0),
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FailingExportCreatesNotification()
        {
            _mockExporter.Setup(
                e => e.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));

            WhenExportIsExecuted();

            Assert.IsTrue(_notificationHelper.IsRequestRaised);
        }

        [Test]
        public void FailingExportNotificationHasCorrectMessage()
        {
            _mockExporter.Setup(
                e => e.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));

            WhenExportIsExecuted();

            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = Properties.UploadWizard.window_title,
                Message = Properties.UploadWizard.ExportFail + ":\nTEST",
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FailingExportLogsError()
        {
            var exception = new AssertException("TEST");
            _mockExporter.Setup(
                e => e.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<IPublisher>())).Throws(exception);

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
        public void ShouldExportAllEventsThatStartetBeforeOrAtExportTime()
        {
            _testDateUtils.Now = DateTime.Now;
            var logContent = new[]
            {
                new TestIDEEvent {TriggeredAt = _testDateUtils.Now.AddSeconds(-1)},
                new TestIDEEvent {TriggeredAt = _testDateUtils.Now},
                new TestIDEEvent {TriggeredAt = _testDateUtils.Now.AddSeconds(1)}
            };
            _mockLogs[0].Setup(log => log.ReadAll()).Returns(logContent);
            var expectedExport = logContent.Take(2);
            IEnumerable<IDEEvent> actualExport = null;
            _mockExporter.Setup(
                e => e.Export(It.IsAny<IList<IDEEvent>>(), It.IsAny<IPublisher>()))
                         .Callback<IEnumerable<IDEEvent>, IPublisher>((export, p) => actualExport = export);

            WhenExportIsExecuted();

            Assert.AreEqual(expectedExport, actualExport);
            _mockLogFileManager.Verify(lm => lm.DeleteLogsOlderThan(_testDateUtils.Now));
        }

        private void WhenExportIsExecuted()
        {
            // ReSharper disable once IntroduceOptionalParameters.Local
            WhenExportIsExecuted(UploadWizard.ExportType.HttpUpload);
        }

        private void WaitUntilViewModelIsIdle()
        {
            AsyncTestHelper.WaitForCondition(() => !_uut.IsBusy);
        }

        private void WhenExportIsExecuted(UploadWizard.ExportType exportType)
        {
            _uut.Export(exportType);
            WaitUntilViewModelIsIdle();
        }
    }
}
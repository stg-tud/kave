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
 *    - Sebastian Proksch
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using JetBrains;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Tests.Interactivity;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.FeedbackViewModel
{
    [TestFixture]
    internal class ExportCommandTest
    {
        private const string TestUploadUrl = "http://foo.bar";
        private Mock<ILogManager<IDEEvent>> _mockLogFileManager;
        private List<Mock<ILog<IDEEvent>>> _mockLogs;
        private VsFeedbackGenerator.SessionManager.FeedbackViewModel _uut;
        private InteractionRequestTestHelper<UploadWizard.UploadOptions> _requestHelper;
        private Mock<IExporter> _mockExporter;
        private Mock<ISettingsStore> _mockSettingStore;
        private InteractionRequestTestHelper<Notification> _notificationHelper;
        private TestDateUtils _testDateUtils;
        private Mock<IIoUtils> _mockIoUtils;

        [SetUp]
        public void SetUp()
        {
            _mockIoUtils = new Mock<IIoUtils>();
            Registry.RegisterComponent(_mockIoUtils.Object);

            _mockExporter = new Mock<IExporter>();

            var mockLog1 = new Mock<ILog<IDEEvent>>();
            mockLog1.Setup(log => log.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            var mockLog2 = new Mock<ILog<IDEEvent>>();
            mockLog2.Setup(log => log.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            var mockLog3 = new Mock<ILog<IDEEvent>>();
            mockLog3.Setup(log => log.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            _mockLogs = new List<Mock<ILog<IDEEvent>>> {mockLog1, mockLog2, mockLog3};

            _mockLogFileManager = new Mock<ILogManager<IDEEvent>>();
            _mockLogFileManager.Setup(mgr => mgr.GetLogs()).Returns(_mockLogs.Select(m => m.Object));

            _mockSettingStore = new Mock<ISettingsStore>();
            _mockSettingStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(new UploadSettings());
            _mockSettingStore.Setup(store => store.GetSettings<ExportSettings>())
                             .Returns(new ExportSettings {UploadUrl = TestUploadUrl});

            _testDateUtils = new TestDateUtils();
            _uut = new VsFeedbackGenerator.SessionManager.FeedbackViewModel(
                _mockLogFileManager.Object,
                _mockExporter.Object,
                _mockSettingStore.Object,
                _testDateUtils);
            _uut.Refresh();
            while (_uut.Refreshing)
            {
                Thread.Sleep(5);
            }

            _requestHelper = _uut.UploadOptionsRequest.NewTestHelper();
            _notificationHelper = _uut.NotificationRequest.NewTestHelper();
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }

        [Test]
        public void ExecutingTheCommandRaisesInteractionRequest()
        {
            _uut.ExportCommand.Execute(null);
            Assert.IsTrue(_requestHelper.IsRequestRaised);
        }

        [Test]
        public void SelectingZipExportInvokesZipExport()
        {
            WhenExportIsExecuted(UploadWizard.UploadOptions.ExportType.ZipFile);

            _mockExporter.Verify(
                exporter => exporter.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<FilePublisher>()));
        }

        [Test]
        public void SelectingHttpExportInvokesHttpExport()
        {
            WhenExportIsExecuted(UploadWizard.UploadOptions.ExportType.HttpUpload);

            _mockExporter.Verify(
                exporter => exporter.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<HttpPublisher>()));
        }

        [Test]
        public void ShouldUseUploadUrlFromSettingsForUpload()
        {
            HttpPublisher httpPublisher = null;
            _mockExporter.Setup(
                exporter => exporter.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<HttpPublisher>()))
                         .Callback<IEnumerable<IDEEvent>, IPublisher>(
                             (evts, publisher) => httpPublisher = (HttpPublisher) publisher);

            WhenExportIsExecuted(UploadWizard.UploadOptions.ExportType.HttpUpload);
            try
            {
                // Sadly there's currently no clean way to find out what Url is passed to the publisher. Therefore, we
                // invoke the publisher and rely on that it calls IIoUtils.TransferByHttp and then fails because the
                // utils return null.
                httpPublisher.Publish(new MemoryStream());
            }
            catch (NullReferenceException) {}

            _mockIoUtils.Verify(ioUtils => ioUtils.TransferByHttp(It.IsAny<HttpContent>(), new Uri(TestUploadUrl), 5));
        }

        [Test]
        public void SuccessfulExportCreatesNotification()
        {
            WhenExportIsExecuted();
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
        }

        [Test]
        public void SuccessfulExportNotificationHasCorrectMessage()
        {
            WhenExportIsExecuted();
            var actual = _notificationHelper.Context;
            // TODO @Sven: extend setup to include some events that are exported here
            // TODO @Seb: help sven with above task
            var expected = new Notification
            {
                Caption = Properties.UploadWizard.window_title,
                Message = Properties.SessionManager.ExportSuccess.FormatEx(0)
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FailingExportCreatesNotification()
        {
            _mockExporter.Setup(e => e.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));
            WhenExportIsExecuted();
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
        }

        [Test]
        public void FailingExportNotificationHasCorrectMessage()
        {
            _mockExporter.Setup(e => e.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));
            WhenExportIsExecuted();
            var actual = _notificationHelper.Context;
            var expected = new Notification
            {
                Caption = Properties.UploadWizard.window_title,
                Message = Properties.SessionManager.ExportFail + ":\nTEST"
            };
            Assert.AreEqual(expected, actual);
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
        public void NotSelectingAnyExportMethodShouldResultInNoAction()
        {
            WhenExportIsExecuted(null);

            _mockExporter.Verify(
                exporter => exporter.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<IPublisher>()),
                Times.Never);
            Assert.IsFalse(_notificationHelper.IsRequestRaised);
        }

        private void WhenExportIsExecuted()
        {
            // ReSharper disable once IntroduceOptionalParameters.Local
            WhenExportIsExecuted(UploadWizard.UploadOptions.ExportType.HttpUpload);
        }

        private void WhenExportIsExecuted(UploadWizard.UploadOptions.ExportType? exportType)
        {
            _uut.ExportCommand.Execute(null);
            _requestHelper.Context.Type = exportType;
            _requestHelper.Callback();
        }
    }
}
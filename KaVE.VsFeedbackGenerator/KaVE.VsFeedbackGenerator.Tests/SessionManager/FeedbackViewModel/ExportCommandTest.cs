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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Tests.Interactivity;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.FeedbackViewModel
{
    [TestFixture]
    class ExportCommandTest
    {
        private Mock<ILogManager<IDEEvent>> _mockLogFileManager;
        private List<Mock<ILog<IDEEvent>>> _mockLogs;
        private VsFeedbackGenerator.SessionManager.FeedbackViewModel _uut;
        private InteractionRequestTestHelper<UploadWizard.UploadOptions> _requestHelper;
        private Mock<IExporter> _mockExporter;
        private Mock<ISettingsStore> _mockSettingStore;
        private InteractionRequestTestHelper<Notification> _notificationHelper;

        [SetUp]
        public void SetUp()
        {
            Registry.RegisterComponent(new Mock<IIoUtils>().Object);

            _mockExporter = new Mock<IExporter>();

            var mockLog1 = new Mock<ILog<IDEEvent>>();
            mockLog1.Setup(log => log.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            var mockLog2 = new Mock<ILog<IDEEvent>>();
            mockLog2.Setup(log => log.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            var mockLog3 = new Mock<ILog<IDEEvent>>();
            mockLog3.Setup(log => log.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            _mockLogs = new List<Mock<ILog<IDEEvent>>> { mockLog1, mockLog2, mockLog3 };

            _mockLogFileManager = new Mock<ILogManager<IDEEvent>>();
            _mockLogFileManager.Setup(mgr => mgr.GetLogs()).Returns(_mockLogs.Select(m => m.Object));

            _mockSettingStore = new Mock<ISettingsStore>();
            _mockSettingStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(new UploadSettings());
            var exportSettings = new ExportSettings {UploadUrl = "http://foo.bar"};
            _mockSettingStore.Setup(store => store.GetSettings<ExportSettings>()).Returns(exportSettings);

            _uut = new VsFeedbackGenerator.SessionManager.FeedbackViewModel(_mockLogFileManager.Object, _mockSettingStore.Object, _mockExporter.Object);
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
            _uut.ExportCommand.Execute(null);
            _requestHelper.Context.Type = UploadWizard.UploadOptions.ExportType.ZipFile;
            _requestHelper.Callback();

            _mockExporter.Verify(exporter => exporter.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<FilePublisher>()));
        }

        [Test]
        public void SelectingZipExportInvokesHttpExport()
        {
            _uut.ExportCommand.Execute(null);
            _requestHelper.Context.Type = UploadWizard.UploadOptions.ExportType.HttpUpload;
            _requestHelper.Callback();

            _mockExporter.Verify(exporter => exporter.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<HttpPublisher>()));
        }

        [Test]
        public void SuccessfulExportCreatesNotification()
        {
            _uut.ExportCommand.Execute(null);
            _requestHelper.Context.Type = UploadWizard.UploadOptions.ExportType.HttpUpload;
            _requestHelper.Callback();
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
        }

        [Test]
        public void SuccessfulExportNotificationHasCorrectMessage()
        {
            _uut.ExportCommand.Execute(null);
            _requestHelper.Context.Type = UploadWizard.UploadOptions.ExportType.HttpUpload;
            _requestHelper.Callback();
            var actual = _notificationHelper.Context;
            // TODO @Sven: extend setup to include some events that are exported here
            // TODO @Seb: help sven with above task
            var expected = new Notification { Message = Properties.SessionManager.ExportSuccess.FormatEx(0) };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FailingExportCreatesNotification()
        {
            _mockExporter.Setup(e => e.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));
            _uut.ExportCommand.Execute(null);
            _requestHelper.Context.Type = UploadWizard.UploadOptions.ExportType.HttpUpload;
            _requestHelper.Callback();
            Assert.IsTrue(_notificationHelper.IsRequestRaised);
        }

        [Test]
        public void FailingExportNotificationHasCorrectMessage()
        {
            _mockExporter.Setup(e => e.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<IPublisher>()))
                         .Throws(new AssertException("TEST"));
            _uut.ExportCommand.Execute(null);
            _requestHelper.Context.Type = UploadWizard.UploadOptions.ExportType.HttpUpload;
            _requestHelper.Callback();
            var actual = _notificationHelper.Context;
            var expected = new Notification { Message = Properties.SessionManager.ExportFail + ":\nTEST" };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NotSelectingAnyExportMethodShouldResultInNoAction()
        {
            _uut.ExportCommand.Execute(null);
            _requestHelper.Callback();

            _mockExporter.Verify(exporter => exporter.Export(It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<IPublisher>()), Times.Never);
            Assert.IsFalse(_notificationHelper.IsRequestRaised);
        }

        // TODO Write test that checks update of UploadSettings.LastUploadDate
        // TODO Write test that checks usage of ExportSettings.UploadUrl

    }
}

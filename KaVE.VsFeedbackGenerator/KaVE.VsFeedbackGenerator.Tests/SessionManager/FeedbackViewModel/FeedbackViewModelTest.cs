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
 *    - Dennis Albrecht
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Threading;
using KaVE.Model.Events;
using KaVE.TestUtils;
using KaVE.TestUtils.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Export;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.FeedbackViewModel
{
    [TestFixture]
    internal class FeedbackViewModelTest
    {
        private Mock<ILogManager<IDEEvent>> _mockLogManager;
        private Mock<ILogger> _mockLogger;
        private Mock<IActionManager> _mockActionManager;

        private VsFeedbackGenerator.SessionManager.FeedbackViewModel _uut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger>();
            Registry.RegisterComponent(_mockLogger.Object);

            _mockLogManager = new Mock<ILogManager<IDEEvent>>();

            var mockThreading = new Mock<IThreading>();
            _mockActionManager = new Mock<IActionManager>();
            Registry.RegisterComponent(mockThreading.Object);

            _uut = new VsFeedbackGenerator.SessionManager.FeedbackViewModel(
                _mockLogManager.Object,
                _mockActionManager.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldSetRefreshBusyMessage()
        {
            _uut.Refresh();

            Assert.AreEqual(Properties.SessionManager.Refreshing, _uut.BusyMessage);
        }

        [Test]
        public void ShouldGetAllLogsFromLogManagerOnRefresh()
        {
            _uut.Refresh();
            WaitForRefreshToFinish();

            _mockLogManager.Verify(lm => lm.GetLogs());
        }

        [Test]
        public void ShouldCreateOneSessionSubViewModelPerLogOnRefresh()
        {
            var logs = new[]
            {
                MockLog(),
                MockLog()
            };
            _mockLogManager.Setup(lm => lm.GetLogs()).Returns(logs);

            _uut.Refresh();
            WaitForRefreshToFinish();

            var actuals = _uut.Sessions.ToList();
            Assert.AreEqual(2, actuals.Count);
            Assert.AreEqual(logs[0], actuals[0].Log);
            Assert.AreEqual(logs[1], actuals[1].Log);
        }

        [Test]
        public void ShouldDisplayNothingAndLogErrorWhenRefreshFails()
        {
            var exception = new AssertException("test exception");
            _mockLogManager.Setup(lm => lm.GetLogs()).Throws(exception);

            _uut.Refresh();
            WaitForRefreshToFinish();

            Assert.IsFalse(_uut.Sessions.Any());
            _mockLogger.Verify(l => l.Error(exception));
        }

        [Test]
        public void ShouldInvokeExportWizardOnExport()
        {
            _uut.ExportCommand.Execute(null);

            // I couldn't find a better way to assert invocation. If you know/find one, let me know! (Sven)
            _mockActionManager.Verify(am => am.GetExecutableAction(UploadWizardActionHandler.ActionId));
        }

        [Test]
        public void ShouldRefreshOnLogManagerLogsChangedEvent()
        {
            _mockLogManager.Raise(lm => lm.LogsChanged += null, _mockLogManager.Object, new EventArgs());

            WaitForRefreshToFinish();

            _mockLogManager.Verify(lm => lm.GetLogs());
        }

        [Test, Ignore]
        public void ShouldInvokeSelectedSessionEventIfOneSessionWasSelectedPreviously()
        {
            var somePointInTime = DateTime.Now;
            var selectedSessionEventWasCalled = false;
            var sessions = new List<ILog<IDEEvent>>
            {
                MockLog(somePointInTime),
                MockLog(somePointInTime.AddDays(1)),
                MockLog(somePointInTime.AddDays(2)),
                MockLog(somePointInTime.AddDays(3))
            };
            InitializeFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> {new SessionViewModel(sessions[0])};
            _uut.SelectedSessionAfterRefresh += (o, model) => { selectedSessionEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsTrue(selectedSessionEventWasCalled);
        }

        [Test, Ignore]
        public void ShouldNotInvokeSelectedSessionEventIfNoSessionsWereSelectedPreviously()
        {
            var somePointInTime = DateTime.Now;
            var selectedSessionEventWasCalled = false;
            var sessions = new List<ILog<IDEEvent>>
            {
                MockLog(somePointInTime),
                MockLog(somePointInTime.AddDays(1)),
                MockLog(somePointInTime.AddDays(2)),
                MockLog(somePointInTime.AddDays(3))
            };
            InitializeFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel>();
            _uut.SelectedSessionAfterRefresh += (o, model) => { selectedSessionEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsFalse(selectedSessionEventWasCalled);
        }

        [Test, Ignore]
        public void ShouldNotInvokeSelectedSessionEventIfMultipleSessionsWereSelectedPreviously()
        {
            var somePointInTime = DateTime.Now;
            var selectedSessionEventWasCalled = false;
            var sessions = new List<ILog<IDEEvent>>
            {
                MockLog(somePointInTime),
                MockLog(somePointInTime.AddDays(1)),
                MockLog(somePointInTime.AddDays(2)),
                MockLog(somePointInTime.AddDays(3))
            };
            InitializeFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel>
            {
                new SessionViewModel(sessions[0]),
                new SessionViewModel(sessions[1])
            };
            _uut.SelectedSessionAfterRefresh += (o, model) => { selectedSessionEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsFalse(selectedSessionEventWasCalled);
        }

        [Test, Ignore]
        public void ShouldInvokeSelectedEventEventIfOneEventWasSelectedPreviously()
        {
            var somePointInTime = DateTime.Now;
            var selectedEventEventWasCalled = false;
            var events = new List<IDEEvent>
            {
                new TestIDEEvent {TestProperty = "Event 1"},
                new TestIDEEvent {TestProperty = "Event 2"},
                new TestIDEEvent {TestProperty = "Event 3"}
            };
            var sessions = new List<ILog<IDEEvent>>
            {
                MockLog(somePointInTime, events),
                MockLog(somePointInTime.AddDays(1)),
                MockLog(somePointInTime.AddDays(2)),
                MockLog(somePointInTime.AddDays(3))
            };
            InitializeFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> {new SessionViewModel(sessions[0])};
            Debug.Assert(_uut.SingleSelectedSession != null, "_uut.SingleSelectedSession != null");
            _uut.SingleSelectedSession.SelectedEvents = new List<EventViewModel>
            {
                new EventViewModel(events[0])
            };
            _uut.SelectedSessionAfterRefresh += (o, model) => { };
            _uut.SelectedEventAfterRefresh += (o, model) => { selectedEventEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsTrue(selectedEventEventWasCalled);
        }

        [Test, Ignore]
        public void ShouldNotInvokeSelectedEventEventIfNoEventsWereSelectedPreviously()
        {
            var somePointInTime = DateTime.Now;
            var selectedEventEventWasCalled = false;
            var events = new List<IDEEvent>
            {
                new TestIDEEvent {TestProperty = "Event 1"},
                new TestIDEEvent {TestProperty = "Event 2"},
                new TestIDEEvent {TestProperty = "Event 3"}
            };
            var sessions = new List<ILog<IDEEvent>>
            {
                MockLog(somePointInTime, events),
                MockLog(somePointInTime.AddDays(1)),
                MockLog(somePointInTime.AddDays(2)),
                MockLog(somePointInTime.AddDays(3))
            };
            InitializeFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> { new SessionViewModel(sessions[0]) };
            Debug.Assert(_uut.SingleSelectedSession != null, "_uut.SingleSelectedSession != null");
            _uut.SingleSelectedSession.SelectedEvents = new List<EventViewModel>();
            _uut.SelectedSessionAfterRefresh += (o, model) => { };
            _uut.SelectedEventAfterRefresh += (o, model) => { selectedEventEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsFalse(selectedEventEventWasCalled);
        }

        [Test, Ignore]
        public void ShouldNotInvokeSelectedEventEventIfMultipleEventsWereSelectedPreviously()
        {
            var somePointInTime = DateTime.Now;
            var selectedEventEventWasCalled = false;
            var events = new List<IDEEvent>
            {
                new TestIDEEvent {TestProperty = "Event 1"},
                new TestIDEEvent {TestProperty = "Event 2"},
                new TestIDEEvent {TestProperty = "Event 3"}
            };
            var sessions = new List<ILog<IDEEvent>>
            {
                MockLog(somePointInTime, events),
                MockLog(somePointInTime.AddDays(1)),
                MockLog(somePointInTime.AddDays(2)),
                MockLog(somePointInTime.AddDays(3))
            };
            InitializeFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> { new SessionViewModel(sessions[0]) };
            Debug.Assert(_uut.SingleSelectedSession != null, "_uut.SingleSelectedSession != null");
            _uut.SingleSelectedSession.SelectedEvents = new List<EventViewModel>
            {
                new EventViewModel(events[0]),
                new EventViewModel(events[1])
            };
            _uut.SelectedSessionAfterRefresh += (o, model) => { };
            _uut.SelectedEventAfterRefresh += (o, model) => { selectedEventEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsFalse(selectedEventEventWasCalled);
        }

        private void InitializeFeedbackViewModelWithSessions(IEnumerable<ILog<IDEEvent>> sessions)
        {
            _mockLogManager.Setup(m => m.GetLogs()).Returns(sessions);
            _uut.Refresh();
            WaitForRefreshToFinish();
        }

        private void RefreshFeedbackViewModelWithSessions(IEnumerable<ILog<IDEEvent>> sessions)
        {
            _mockLogManager.Setup(m => m.GetLogs()).Returns(sessions);
            _uut.Refresh();
            WaitForRefreshToFinish();
        }

        private static ILog<IDEEvent> MockLog()
        {
            var log = new Mock<ILog<IDEEvent>>();
            log.Setup(l => l.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            return log.Object;
        }

        private static ILog<IDEEvent> MockLog(DateTime startTime)
        {
            var log = new Mock<ILog<IDEEvent>>();
            log.Setup(l => l.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
            log.Setup(l => l.Date).Returns(startTime);
            return log.Object;
        }

        private static ILog<IDEEvent> MockLog(DateTime startTime, IEnumerable<IDEEvent> events)
        {
            var log = new Mock<ILog<IDEEvent>>();
            var reader = new Mock<ILogReader<IDEEvent>>();
            reader.Setup(r => r.ReadAll()).Returns(events);
            log.Setup(l => l.NewLogReader()).Returns(reader.Object);
            log.Setup(l => l.Date).Returns(startTime);
            return log.Object;
        }

        private void WaitForRefreshToFinish()
        {
            AsyncTestHelper.WaitForCondition(() => !_uut.IsBusy);
        }
    }
}
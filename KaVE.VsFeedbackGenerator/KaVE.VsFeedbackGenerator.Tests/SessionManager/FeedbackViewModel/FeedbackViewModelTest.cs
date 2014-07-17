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
using JetBrains.Threading;
using KaVE.Model.Events;
using KaVE.TestUtils;
using KaVE.TestUtils.Model.Events;
using KaVE.Utils;
using KaVE.Utils.Assertion;
using KaVE.Utils.Reflection;
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

        private VsFeedbackGenerator.SessionManager.FeedbackViewModel _uut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger>();
            Registry.RegisterComponent(_mockLogger.Object);

            _mockLogManager = new Mock<ILogManager<IDEEvent>>();

            var mockThreading = new Mock<IThreading>();
            Registry.RegisterComponent(mockThreading.Object);

            _uut = new VsFeedbackGenerator.SessionManager.FeedbackViewModel(
                _mockLogManager.Object);
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
        public void ShouldFireHasEventsPropertyChangedAfterRefresh()
        {
            var hasEventsChanged = false;
            _uut.OnPropertyChanged(self => self.AreAnyEventsPresent, newVal => hasEventsChanged = true);

            _uut.Refresh();
            WaitForRefreshToFinish();

            Assert.IsTrue(hasEventsChanged);
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
        public void ShouldRefreshOnLogManagerLogsChangedEvent()
        {
            _mockLogManager.Raise(lm => lm.LogsChanged += null, _mockLogManager.Object, new EventArgs());

            WaitForRefreshToFinish();

            _mockLogManager.Verify(lm => lm.GetLogs());
        }

        [Test]
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
            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> {new SessionViewModel(sessions[0])};
            // TODO @Dennis: Testen ob richtiges viewmodel kommt
            _uut.SessionSelection += (o, model) => { selectedSessionEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsTrue(selectedSessionEventWasCalled);
        }

        [Test]
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
            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel>();
            _uut.SessionSelection += (o, model) => { selectedSessionEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsFalse(selectedSessionEventWasCalled);
        }

        [Test]
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
            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel>
            {
                new SessionViewModel(sessions[0]),
                new SessionViewModel(sessions[1])
            };
            _uut.SessionSelection += (o, model) => { selectedSessionEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            // TODO @Dennis: implement for multiselect (Add ;))
            Assert.IsFalse(selectedSessionEventWasCalled);
        }

        [Test]
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
            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> {new SessionViewModel(sessions[0])};
            // ReSharper disable once PossibleNullReferenceException
            _uut.SingleSelectedSession.SelectedEvents = new List<EventViewModel>
            {
                new EventViewModel(events[0])
            };
            // TODO @Dennis: check correct Event
            _uut.EventSelection += (o, model) => { selectedEventEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsTrue(selectedEventEventWasCalled);
        }

        [Test]
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
            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> {new SessionViewModel(sessions[0])};
            // ReSharper disable once PossibleNullReferenceException
            _uut.SingleSelectedSession.SelectedEvents = new List<EventViewModel>();
            _uut.SessionSelection += (o, model) => { };
            _uut.EventSelection += (o, model) => { selectedEventEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsFalse(selectedEventEventWasCalled);
        }

        [Test]
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
            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new List<SessionViewModel> {new SessionViewModel(sessions[0])};
            // ReSharper disable once PossibleNullReferenceException
            _uut.SingleSelectedSession.SelectedEvents = new List<EventViewModel>
            {
                new EventViewModel(events[0]),
                new EventViewModel(events[1])
            };
            _uut.SessionSelection += (o, model) => { };
            _uut.EventSelection += (o, model) => { selectedEventEventWasCalled = true; };
            RefreshFeedbackViewModelWithSessions(sessions);
            Assert.IsFalse(selectedEventEventWasCalled);
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
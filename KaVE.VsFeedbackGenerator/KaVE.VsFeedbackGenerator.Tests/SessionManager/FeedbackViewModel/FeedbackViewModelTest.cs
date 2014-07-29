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
using System.Linq;
using JetBrains.Threading;
using KaVE.Model.Events;
using KaVE.TestUtils;
using KaVE.TestUtils.Model.Events;
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

            Assert.IsTrue(_uut.BusyMessage.StartsWith(Properties.SessionManager.Refreshing));
        }

        [Test]
        public void ShouldGetAllLogsFromLogManagerOnRefresh()
        {
            RefreshFeedbackViewModel();

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

            RefreshFeedbackViewModel();

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

            RefreshFeedbackViewModel();

            Assert.IsTrue(hasEventsChanged);
        }

        [Test]
        public void ShouldDisplayNothingAndLogErrorWhenRefreshFails()
        {
            var exception = new AssertException("test exception");
            _mockLogManager.Setup(lm => lm.GetLogs()).Throws(exception);

            RefreshFeedbackViewModel();

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
        public void ShouldNotInvokeSessionSelectionEventIfNoSessionsWereSelectedPreviously()
        {
            var sessions = MockLogs(4);
            var expectedSelection = new List<SessionViewModel>();
            IList<SessionViewModel> actualSelection = null;

            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = expectedSelection;
            _uut.SessionSelection += (o, models) => { actualSelection = models; };
            RefreshFeedbackViewModelWithSessions(sessions);

            Assert.IsNull(actualSelection);
        }

        [TestCase(1), TestCase(2)]
        public void ShouldInvokeSessionSelectionEventIfSessionsWereSelectedPreviously(int numberOfSelections)
        {
            var sessions = MockLogs(4);
            var expectedSelection = sessions.Take(numberOfSelections).Select(s => new SessionViewModel(s)).ToList();
            IList<SessionViewModel> actualSelection = null;

            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = expectedSelection;
            _uut.SessionSelection += (o, models) => { actualSelection = models; };
            RefreshFeedbackViewModelWithSessions(sessions);

            CollectionAssert.AreEquivalent(expectedSelection, actualSelection);
        }

        [Test]
        public void ShouldNotInvokeEventSelectionEventIfNoEventsWereSelectedPreviously()
        {
            var sessions = MockLogs(4, 3);
            var expectedSelection = new List<EventViewModel>();
            IList<EventViewModel> actualSelection = null;

            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new [] {new SessionViewModel(sessions[0])};
            // ReSharper disable once PossibleNullReferenceException
            _uut.SingleSelectedSession.SelectedEvents = expectedSelection;
            _uut.EventSelection += (o, models) => { actualSelection = models; };
            RefreshFeedbackViewModelWithSessions(sessions);

            Assert.IsNull(actualSelection);
        }

        [TestCase(1), TestCase(2)]
        public void ShouldInvokeEventSelectionEventIfEventsWereSelectedPreviously(int numberOfSelection)
        {
            var sessions = MockLogs(4, 3);
            var selectedSession = new SessionViewModel(sessions[0]);
            var expectedSelection = selectedSession.Events.Take(numberOfSelection).ToList();
            IList<EventViewModel> actualSelection = null;

            RefreshFeedbackViewModelWithSessions(sessions);
            _uut.SelectedSessions = new [] {selectedSession};
            // ReSharper disable once PossibleNullReferenceException
            _uut.SingleSelectedSession.SelectedEvents = expectedSelection;
            _uut.EventSelection += (o, models) => { actualSelection = models; };
            RefreshFeedbackViewModelWithSessions(sessions);

            CollectionAssert.AreEquivalent(expectedSelection, actualSelection);
        }

        private static List<ILog<IDEEvent>> MockLogs(int numberOfLogs, int numberOfEventsPerLog = 0)
        {
            var somePointInTime = DateTime.Now;
            return
                Enumerable.Range(0, numberOfLogs)
                          .Select(i => MockLog(somePointInTime.AddDays(i), MockEvents(i + "_", numberOfEventsPerLog)))
                          .ToList();
        }

        private static IEnumerable<IDEEvent> MockEvents(string prefix, int number)
        {
            return Enumerable.Range(0, number).Select(i => (IDEEvent) new TestIDEEvent {TestProperty = prefix + i});
        }

        private void RefreshFeedbackViewModelWithSessions(IEnumerable<ILog<IDEEvent>> sessions)
        {
            _mockLogManager.Setup(m => m.GetLogs()).Returns(sessions);
            RefreshFeedbackViewModel();
        }

        private void RefreshFeedbackViewModel()
        {
            _uut.Refresh();
            WaitForRefreshToFinish();
        }

        private static ILog<IDEEvent> MockLog()
        {
            var log = new Mock<ILog<IDEEvent>>();
            log.Setup(l => l.NewLogReader()).Returns(new Mock<ILogReader<IDEEvent>>().Object);
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
﻿/*
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

using System.Collections.Generic;
using System.Linq;
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
using Fix = KaVE.VsFeedbackGenerator.Tests.SessionManager.FeedbackViewModelTestSuite.LogTestHelper;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.FeedbackViewModelTestSuite
{
    [TestFixture]
    internal class FeedbackViewModelTest
    {
        private Mock<ILogManager> _mockLogManager;
        private Mock<ILogger> _mockLogger;

        private FeedbackViewModel _uut;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger>();
            Registry.RegisterComponent(_mockLogger.Object);

            _mockLogManager = new Mock<ILogManager>();

            _uut = new FeedbackViewModel(_mockLogManager.Object);
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
            WhenViewModelIsRefreshedWithSessions(new ILog[0]);

            _mockLogManager.Verify(lm => lm.Logs);
        }

        [Test]
        public void ShouldCreateOneSessionSubViewModelPerLogOnRefresh()
        {
            var expecteds = Fix.SomeLogs(2);
            WhenViewModelIsRefreshedWithSessions(expecteds);

            var actuals = _uut.Sessions.Select(session => session.Log);
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void ShouldFireHasEventsPropertyChangedAfterRefresh()
        {
            var hasEventsChanged = false;
            _uut.OnPropertyChanged(self => self.AreAnyEventsPresent, newVal => hasEventsChanged = true);

            WhenViewModelIsRefreshedWithSessions(new ILog[0]);

            Assert.IsTrue(hasEventsChanged);
        }

        [Test]
        public void ShouldDisplayNothingAndLogErrorWhenRefreshFails()
        {
            var exception = new AssertException("test exception");
            _mockLogManager.Setup(lm => lm.Logs).Throws(exception);

            WhenViewModelIsRefreshed();

            Assert.IsEmpty(_uut.Sessions);
            _mockLogger.Verify(l => l.Error(ItIsException.With("refresh failed", exception)));
        }

        [Test]
        public void ShouldAddSessionViewIfLogIsCreated()
        {
            Assert.IsEmpty(_uut.Sessions);

            var log = Fix.SomeLog();
            _mockLogManager.Raise(lm => lm.LogCreated += null, log);

            Assert.AreEqual(1, _uut.Sessions.Count());
            Assert.AreEqual(log, _uut.Sessions.First().Log);
        }

        [Test(Description = "UI doesn't update otherwise, because log creation is not triggered by user interaction")]
        public void ShouldRaiseSessionsPropertyChangedAfterLogIsCreated()
        {
            var sessionsChanged = false;
            _uut.OnPropertyChanged(uut => uut.Sessions, sessions => sessionsChanged = true);

            _mockLogManager.Raise(lm => lm.LogCreated += null, Fix.SomeLog());

            Assert.IsTrue(sessionsChanged);
        }

        [Test]
        public void ShouldRemoveSessionViewIfLogIsDeleted()
        {
            var mockLog = new Mock<ILog>();
            WhenViewModelIsRefreshedWithSessions(new []{mockLog.Object});

            Assert.IsNotEmpty(_uut.Sessions);

            mockLog.Raise(log => log.Deleted += null, mockLog.Object);

            Assert.IsEmpty(_uut.Sessions);
        }

        [Test]
        public void ShouldNotSetSelectedSessionsAfterRefreshIfNoSelectionExistedBefore()
        {
            var sessions = Fix.SomeLogs(4);
            WhenViewModelIsRefreshedWithSessions(sessions);
            var eventInvoked = false;
            _uut.OnPropertyChanged(m => m.SelectedSessions, selection => eventInvoked = true);

            WhenViewModelIsRefreshedWithSessions(sessions);

            Assert.IsFalse(eventInvoked);
        }

        [TestCase(1), TestCase(2)]
        public void ShouldInvokeSessionSelectionEventIfSessionsWereSelectedPreviously(int numberOfSelections)
        {
            var sessions = Fix.SomeLogs(4);
            WhenViewModelIsRefreshedWithSessions(sessions);
            var expectedSelection = _uut.Sessions.Take(numberOfSelections).ToList();
            _uut.SelectedSessions = expectedSelection;
            ICollection<SessionViewModel> actualSelection = null;
            _uut.OnPropertyChanged(m => m.SelectedSessions, selection => actualSelection = selection);

            WhenViewModelIsRefreshedWithSessions(sessions);

            CollectionAssert.AreEquivalent(expectedSelection, actualSelection);
        }

        [Test]
        public void ShouldNotInvokeEventSelectionEventIfNoEventsWereSelectedPreviously()
        {
            var sessions = new[] {Fix.SomeLog(IDEEventTestFactory.SomeEvents(4))};
            WhenViewModelIsRefreshedWithSessions(sessions);
            _uut.SelectedSessions = new[] {_uut.Sessions.First()};
            var eventInvoked = false;
            _uut.SingleSelectedSession.OnPropertyChanged(m => m.SelectedEvents, selection => eventInvoked = true);

            WhenViewModelIsRefreshedWithSessions(sessions);

            Assert.IsFalse(eventInvoked);
        }

        [TestCase(1), TestCase(2)]
        public void ShouldInvokeEventSelectionEventIfEventsWereSelectedPreviously(int numberOfSelection)
        {
            var sessions = new[] {Fix.SomeLog(IDEEventTestFactory.SomeEvents(3))};
            WhenViewModelIsRefreshedWithSessions(sessions);
            _uut.SelectedSessions = new[] {_uut.Sessions.First()};
            WhenEventsOfSingleSelectedSessionAreLoaded();
            // ReSharper disable once PossibleNullReferenceException
            var expectedSelection = _uut.SingleSelectedSession.Events.Take(numberOfSelection).ToList();
            _uut.SingleSelectedSession.SelectedEvents = expectedSelection;
            ICollection<EventViewModel> actualSelection = null;
            _uut.SingleSelectedSession.OnPropertyChanged(m => m.SelectedEvents, selection => actualSelection = selection);

            WhenViewModelIsRefreshedWithSessions(sessions);
            WhenEventsOfSingleSelectedSessionAreLoaded();

            CollectionAssert.AreEquivalent(expectedSelection, actualSelection);
        }

        private void WhenEventsOfSingleSelectedSessionAreLoaded()
        {
            // ReSharper disable once UnusedVariable
            // ReSharper disable once PossibleNullReferenceException
            var tmp = _uut.SingleSelectedSession.Events;
            WaitForRefreshToFinish();
        }

        private void WhenViewModelIsRefreshedWithSessions(IEnumerable<ILog> sessions)
        {
            _mockLogManager.Setup(m => m.Logs).Returns(sessions);
            WhenViewModelIsRefreshed();
        }

        private void WhenViewModelIsRefreshed()
        {
            _uut.Refresh();
            WaitForRefreshToFinish();
        }

        private void WaitForRefreshToFinish()
        {
            AsyncTestHelper.WaitForCondition(() => !_uut.IsBusy);
        }
    }
}
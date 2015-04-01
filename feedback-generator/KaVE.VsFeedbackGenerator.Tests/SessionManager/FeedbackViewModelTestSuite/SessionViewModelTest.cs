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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.Reflection;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.FeedbackViewModelTestSuite
{
    [TestFixture]
    internal class SessionViewModelTest
    {
        private Mock<ILog> _mockLog;
        private SessionViewModel _uut;
        private Mock<ILogger> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockLog = LogTestHelper.MockLog();
            _uut = new SessionViewModel(_mockLog.Object);

            _mockLogger = new Mock<ILogger>();
            Registry.RegisterComponent(_mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldProvideLogDateAsSessionDate()
        {
            var expected = DateTime.Today;
            _mockLog.Setup(log => log.Date).Returns(expected);

            var actual = _uut.StartDate;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotReadLogOnCreation()
        {
            _mockLog.Verify(log => log.ReadAll(), Times.Never);
        }

        [Test]
        public void ShouldReadEventsFromLog()
        {
            var expected = IDEEventTestFactory.SomeEvents(3);
            GivenLogContainsEvents(expected);

            var actuals = WhenEventViewModelsAreLoaded().Select(evm => evm.Event);

            CollectionAssert.AreEqual(expected, actuals);
        }

        [Test]
        public void ShouldSetBusyMessageOnLoad()
        {
            // ReSharper disable once UnusedVariable
            var tmp = _uut.Events;

            Assert.IsTrue(_uut.BusyMessage.StartsWith(Messages.Loading));
        }

        [Test]
        public void ShouldDisplayNothingIfLoadFails()
        {
            _mockLog.Setup(log => log.ReadAll()).Throws<Exception>();

            var actuals = WhenEventViewModelsAreLoaded();

            Assert.IsEmpty(actuals);
        }

        [Test]
        public void ShouldLogErrorIfLoadFails()
        {
            var exception = new Exception("error");
            _mockLog.Setup(log => log.ReadAll()).Throws(exception);

            WhenEventViewModelsAreLoaded();

            _mockLogger.Verify(l => l.Error(ItIsException.With("could not read log", exception)));
        }

        [Test(Description = "UI doesn't update after refresh, because load is asynchronous and finishes after UI update"
            )]
        public void ShouldRaiseEventsPropertyChangedAfterLoad()
        {
            var actual = false;
            _uut.OnPropertyChanged(uut => uut.Events, events => actual = true);

            WhenEventViewModelsAreLoaded();

            Assert.IsTrue(actual);
        }

        [Test]
        public void ShouldAddNewEventsIfLogEntryIsAppended()
        {
            var expected = IDEEventTestFactory.SomeEvents(3);
            GivenLogContainsEvents(expected.Take(2).ToList());

            WhenEventViewModelsAreLoaded();
            _mockLog.Raise(l => l.EntryAppended += null, expected[2]);

            var actual = _uut.Events.Select(evt => evt.Event);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldReReadLogAfterRefresh()
        {
            GivenLogContainsEvents(IDEEventTestFactory.SomeEvents(1));

            WhenEventViewModelsAreLoaded();

            var expected = IDEEventTestFactory.SomeEvents(3);
            _mockLog.Setup(log => log.ReadAll()).Returns(expected);

            _uut.Refresh();
            var actuals = WhenEventViewModelsAreLoaded().Select(vm => vm.Event);

            CollectionAssert.AreEqual(expected, actuals);
        }

        [Test]
        public void ShouldKeepSelectionOnRefresh()
        {
            GivenLogContainsEvents(IDEEventTestFactory.SomeEvents(3));
            var viewModels = WhenEventViewModelsAreLoaded();
            var expecteds = viewModels.Take(2).ToList();
            _uut.SelectedEvents = expecteds;

            _uut.Refresh();

            WhenEventViewModelsAreLoaded();
            var actuals = _uut.SelectedEvents;

            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        private void GivenLogContainsEvents(IEnumerable<IDEEvent> someEvents)
        {
            _mockLog.Setup(log => log.ReadAll()).Returns(someEvents);
        }

        private IEnumerable<EventViewModel> WhenEventViewModelsAreLoaded()
        {
            // ReSharper disable once UnusedVariable
            var tmp = _uut.Events;
            WaitForLoadToFinish();
            return _uut.Events;
        }

        private void WaitForLoadToFinish()
        {
            AsyncTestHelper.WaitForCondition(() => !_uut.IsBusy);
        }
    }
}
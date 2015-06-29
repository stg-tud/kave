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
using System.Threading;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.FeedbackGenerator.Generators;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators
{
    [TestFixture]
    internal class EventLoggerTest
    {
        private Mock<IMessageBus> _mockMessageBus;
        private Mock<ILogManager> _mockLogManager;
        private Action<IDEEvent> _logHandler;

        private AutoResetEvent _logAppendSignal;
        private List<IDEEvent> _loggedEvents;
        private EventLogger _uut;

        [SetUp]
        public void SetUp()
        {
            _loggedEvents = new List<IDEEvent>();
            _mockMessageBus = new Mock<IMessageBus>();
            _mockMessageBus.Setup(mb => mb.Subscribe(It.IsAny<Action<IDEEvent>>(), null))
                           .Callback<Action<IDEEvent>, Func<IDEEvent, bool>>(
                               (logHandler, dc) => _logHandler = logHandler);

            _logAppendSignal = new AutoResetEvent(false);
            _mockLogManager = new Mock<ILogManager>();
            OnCurrentLogAppend(
                e =>
                {
                    _loggedEvents.Add(e);
                    _logAppendSignal.Set();
                });

            _uut = new EventLogger(_mockMessageBus.Object, _mockLogManager.Object, null);
        }

        private void OnCurrentLogAppend(Action<IDEEvent> action)
        {
            _mockLogManager.Setup(lm => lm.CurrentLog.Append(It.IsAny<IDEEvent>())).Callback(action);
        }

        [Test]
        public void LogsEvent()
        {
            var someEvent = IDEEventTestFactory.SomeEvent();

            WhenLoggerReceives(someEvent);

            AssertAppendToCurrentLog(someEvent);
        }

        [Test]
        public void SupressesNullEvents()
        {
            WhenLoggerReceives((IDEEvent) null);

            AssertNoAppendToCurrentLog();
        }

        [Test]
        public void ShouldNotFailIfLoggingFails()
        {
            var failingEvent = IDEEventTestFactory.SomeEvent();
            var someEvent = IDEEventTestFactory.SomeEvent();

            OnCurrentLogAppend(
                e =>
                {
                    Asserts.Not(failingEvent.Equals(e));
                    _loggedEvents.Add(e);
                    _logAppendSignal.Set();
                });

            WhenLoggerReceives(failingEvent, someEvent);

            AssertAppendToCurrentLog(someEvent);
        }

        [Test, Ignore]
        // RS9 TODO: this tests regularly fails during the build... what's going on here?
        public void ShouldFlushOnShutdown()
        {
            var anEvent = IDEEventTestFactory.SomeEvent();
            var shutdownEvent = new IDEStateEvent {IDELifecyclePhase = IDEStateEvent.LifecyclePhase.Shutdown};

            WhenLoggerReceives(anEvent);
            _uut.Shutdown(shutdownEvent);

            CollectionAssert.AreEqual(new IDEEvent[] {anEvent, shutdownEvent}, _loggedEvents);
        }

        // TODO add tests for event merging logic

        private void WhenLoggerReceives(params IDEEvent[] events)
        {
            events.ForEach(_logHandler);
        }

        private void AssertNoAppendToCurrentLog()
        {
            _logHandler(IDEEventTestFactory.SomeEvent()); // send another event to flush buffer

            Assert.IsFalse(_logAppendSignal.WaitOne(1000), "nothing appended");
        }

        private void AssertAppendToCurrentLog(params IDEEvent[] events)
        {
            _logHandler(IDEEventTestFactory.SomeEvent()); // send another event to flush buffer
            WaitForLogAppend();

            CollectionAssert.AreEqual(events, _loggedEvents);
        }

        private void WaitForLogAppend()
        {
            Assert.IsTrue(_logAppendSignal.WaitOne(5000), "append was not invoked");
        }
    }
}
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

using System.Collections.Generic;
using System.IO;
using System.Threading;
using KaVE.Model.Events;
using KaVE.Model.Events.VisualStudio;
using KaVE.TestUtils.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators
{
    [TestFixture]
    internal class EventLoggerTest
    {
        private Mock<IMessageBus> _mockMessageBus;
        private Mock<ILogManager> _mockLogManager;
        private AutoResetEvent _logAppendSignal;
        private List<IDEEvent> _loggedEvents;

        [SetUp]
        public void SetUp()
        {
            _loggedEvents = new List<IDEEvent>();
            _mockMessageBus = new Mock<IMessageBus>();
            _logAppendSignal = new AutoResetEvent(false);
            _mockLogManager = new Mock<ILogManager>();
            _mockLogManager.Setup(lm => lm.CurrentLog.Append(It.IsAny<IDEEvent>())).Callback<IDEEvent>(
                e =>
                {
                    _loggedEvents.Add(e);
                    _logAppendSignal.Set();
                });
        }

        [Test]
        public void ShouldSubscribeToMessageBus()
        {
            var uut = new EventLogger(_mockMessageBus.Object, _mockLogManager.Object);

            _mockMessageBus.Verify(mb => mb.Subscribe<IDEEvent>(uut.Log, null));
        }

        [Test]
        public void ShouldWriteEventsWithDelayOfOne()
        {
            var anEvent = IDEEventTestFactory.SomeEvent();

            var uut = new EventLogger(_mockMessageBus.Object, _mockLogManager.Object);
            uut.Log(anEvent); // this is written to the buffer
            uut.Log(IDEEventTestFactory.SomeEvent()); // there anEvent should be appended
            WaitForLogAppend();

            CollectionAssert.AreEqual(new[] {anEvent}, _loggedEvents);
        }

        [Test]
        public void ShouldSupressNullEvents()
        {
            var anEvent = IDEEventTestFactory.SomeEvent();

            var uut = new EventLogger(_mockMessageBus.Object, _mockLogManager.Object);
            uut.Log(null);
            uut.Log(anEvent); // this is written to the buffer
            uut.Log(IDEEventTestFactory.SomeEvent()); // there anEvent should be appended
            WaitForLogAppend();

            CollectionAssert.AreEqual(new[] { anEvent }, _loggedEvents);
        }

        [Test]
        public void ShouldNotFailIfLoggingFails()
        {
            var anEvent = IDEEventTestFactory.SomeEvent();

            _mockLogManager.Setup(lm => lm.CurrentLog.Append(It.IsAny<IDEEvent>())).Callback<IDEEvent>(
               e =>
               {
                   Asserts.Not(anEvent.Equals(e));
                   _loggedEvents.Add(e);
                   _logAppendSignal.Set();
               });

            var uut = new EventLogger(_mockMessageBus.Object, _mockLogManager.Object);
            uut.Log(anEvent); // this is written to the buffer
            uut.Log(IDEEventTestFactory.SomeEvent()); // here anEvent is appended, which fails
            uut.Log(IDEEventTestFactory.SomeEvent()); // here an event should be appended

            WaitForLogAppend();
        }

        [Test]
        public void ShouldFlushOnIDEShutdownEvent()
        {
            var anEvent = IDEEventTestFactory.SomeEvent();
            var shutdownEvent = new IDEStateEvent {IDELifecyclePhase = IDEStateEvent.LifecyclePhase.Shutdown};

            var uut = new EventLogger(_mockMessageBus.Object, _mockLogManager.Object);
            uut.Log(anEvent); // this is written to the buffer
            uut.Log(shutdownEvent); // here both events should be appended
            WaitForLogAppend();

            CollectionAssert.AreEqual(new IDEEvent[] { anEvent, shutdownEvent }, _loggedEvents);
        }

        // TODO add tests for event merging logic

        private void WaitForLogAppend()
        {
            Assert.IsTrue(_logAppendSignal.WaitOne(5000), "append was not invoked");
        }
    }
}
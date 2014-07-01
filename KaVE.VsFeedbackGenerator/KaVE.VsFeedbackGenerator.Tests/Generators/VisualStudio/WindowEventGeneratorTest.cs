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

using System.Linq;
using EnvDTE;
using KaVE.Model.Events.VisualStudio;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.Generators.VisualStudio;
using KaVE.VsFeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.VisualStudio
{
    [TestFixture]
    class WindowEventGeneratorTest : VisualStudioEventGeneratorTestBase
    {
        private Mock<WindowEvents> _mockWindowEvents;
        private Window _testWindow;
        private WindowName _testWindowName;
        private WindowEvent _expected;

        [TestFixtureSetUp]
        public void SetUpTestWindow()
        {
            var mockWindow = new Mock<Window>();
            mockWindow.Setup(window => window.Type).Returns(vsWindowType.vsWindowTypeWatch);
            mockWindow.Setup(window => window.Caption).Returns("TestCaption");
            _testWindow = mockWindow.Object;
            _testWindowName = _testWindow.GetName();
        }

        protected override void MockEvents(Mock<Events> mockEvents)
        {
            _mockWindowEvents = new Mock<WindowEvents>();
            // ReSharper disable once UseIndexedProperty
            mockEvents.Setup(events => events.get_WindowEvents(It.IsAny<Window>())).Returns(_mockWindowEvents.Object);
        }

        [SetUp]
        public void SetUp()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new WindowEventGenerator(TestIDESession, TestMessageBus);

            _expected = new WindowEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                Window = _testWindowName,
                TriggeredAt = TestDateUtils.Now,
                TerminatedAt = TestDateUtils.Now
            };
        }

        [Test]
        public void ShouldFireWindowCreationEvent()
        {
            _expected.Action = WindowEvent.WindowAction.Create;

            _mockWindowEvents.Raise(we => we.WindowCreated += null, _testWindow);

            var actual = GetSinglePublished<WindowEvent>();
            Assert.AreEqual(_expected, actual);
        }

        [Test]
        public void ShouldFireWindowActivationEvent()
        {
            _expected.Action = WindowEvent.WindowAction.Activate;

            _mockWindowEvents.Raise(we => we.WindowActivated += null, _testWindow, null);

            var actual = GetSinglePublished<WindowEvent>();
            Assert.AreEqual(_expected, actual);
        }

        [Test]
        public void ShouldFireMoveEventForMovedWindow()
        {
            WhenTestWindowIsMoved();
            var windowEvent = WaitForNewEvent<WindowEvent>();

            Assert.AreEqual(_testWindowName, windowEvent.Window);
            Assert.AreEqual(WindowEvent.WindowAction.Move, windowEvent.Action);
        }

        [Test]
        public void ShouldFireEventAbout150MSAfterMove()
        {
            int actualWaitTime;
            WhenTestWindowIsMoved();
            WaitForNewEvent(out actualWaitTime);
            
            Assert.IsTrue(actualWaitTime >= 150);
        }

        [Test]
        public void ShouldFireSingleEventForMultipleMovesWithin150MS()
        {
            WhenTestWindowIsMoved();
            WaitFor(100);
            WhenTestWindowIsMoved();
            WaitFor(50);
            WhenTestWindowIsMoved();
            WaitForNewEvent();

            Assert.AreEqual(1, GetPublishedEvents().Count());
        }

        [Test]
        public void ShouldFireWindowClosingEvent()
        {
            _expected.Action = WindowEvent.WindowAction.Close;

            _mockWindowEvents.Raise(we => we.WindowClosing += null, _testWindow);

            var actual = GetSinglePublished<WindowEvent>();
            Assert.AreEqual(_expected, actual);
        }

        private void WhenTestWindowIsMoved()
        {
            _mockWindowEvents.Raise(we => we.WindowMoved += null, _testWindow, 0, 0, 0, 0);
        }

        private static void WaitFor(uint milliseconds)
        {
            System.Threading.Thread.Sleep((int) milliseconds);
        }
    }
}

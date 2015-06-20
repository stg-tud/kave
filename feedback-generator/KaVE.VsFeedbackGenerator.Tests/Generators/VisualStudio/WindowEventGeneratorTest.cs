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
using EnvDTE;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio;
using KaVE.VS.FeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio
{
    internal class WindowEventGeneratorTest : VisualStudioEventGeneratorTestBase
    {
        private Mock<WindowEvents> _mockWindowEvents;
        private Window _testWindow;
        private WindowName _testWindowName;
        private WindowEvent _expected;
        private Action _lastScheduledMoveEvent;

        [TestFixtureSetUp]
        public void SetUpTestWindow()
        {
            var mockWindow = new Mock<Window>();
            mockWindow.Setup(window => window.Type).Returns(vsWindowType.vsWindowTypeWatch);
            mockWindow.Setup(window => window.Caption).Returns("TestCaption");
            mockWindow.SetupProperty(window => window.Top);
            mockWindow.SetupProperty(window => window.Left);
            mockWindow.SetupProperty(window => window.Width);
            mockWindow.SetupProperty(window => window.Height);
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
            _lastScheduledMoveEvent = null;
            var mockCallbackManager = new Mock<ICallbackManager>();
            mockCallbackManager.Setup(cm => cm.RegisterCallback(It.IsAny<Action>(), It.IsAny<int>()))
                               .Callback<Action, int>((a, i) => _lastScheduledMoveEvent = a)
                               .Returns(ScheduledAction.NoOp);

            // ReSharper disable once ObjectCreationAsStatement
            new WindowEventGenerator(TestRSEnv, TestMessageBus, mockCallbackManager.Object, TestDateUtils);

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

            WhenWindowIsCreated(_testWindow);

            AssertSinglePublishedWindowEventMeetsExpectations();
        }

        [Test]
        public void ShouldFireWindowActivationEvent()
        {
            _expected.Action = WindowEvent.WindowAction.Activate;

            WhenWindowIsActivated(_testWindow);

            AssertSinglePublishedWindowEventMeetsExpectations();
        }

        [Test]
        public void ShouldNotFireMoveForPreviouslyUnknownWindow()
        {
            WhenWindowIsMoved(_testWindow, downwards: 66, leftwards: 0, addHeight: 0, addWidth: 0);

            Assert.IsNull(_lastScheduledMoveEvent);
        }

        [TestCase(0, 23, 0, 0),
         TestCase(14, 0, 0, 0),
         TestCase(5, 19, 0, 0),
         TestCase(0, 0, 25, 0),
         TestCase(0, 0, 0, 17),
         TestCase(0, 0, 9, 25)]
        public void ShouldNotFireMoveIfWindowIsMovedByLessThan25Pixels(int downwards,
            int leftwards,
            int addHeight,
            int addWidth)
        {
            GivenWindowIsKnown(_testWindow);

            WhenWindowIsMoved(
                _testWindow,
                downwards: downwards,
                leftwards: leftwards,
                addHeight: addHeight,
                addWidth: addWidth);

            Assert.IsNull(_lastScheduledMoveEvent);
        }

        [Test]
        public void ShouldScheduleMoveForPreviouslyCreatedWindow()
        {
            WhenWindowIsCreated(_testWindow);
            WhenWindowIsMoved(_testWindow, downwards: 0, leftwards: 42, addHeight: 0, addWidth: 0);

            Assert.IsNotNull(_lastScheduledMoveEvent);
        }

        [Test]
        public void ShouldScheduleMoveForPreviouslyActivatedWindow()
        {
            WhenWindowIsActivated(_testWindow);
            WhenWindowIsMoved(_testWindow, downwards: 15, leftwards: 26, addHeight: 0, addWidth: 0);

            Assert.IsNotNull(_lastScheduledMoveEvent);
        }

        [TestCase(123, 654, 0, 0),
         TestCase(2, 3, 66, 0)]
        public void ShouldFireMoveEventForMovedWindow(int downwards,
            int leftwards,
            int addHeight,
            int addWidth)
        {
            _expected.Action = WindowEvent.WindowAction.Move;
            GivenWindowIsKnown(_testWindow);

            WhenWindowIsMoved(
                _testWindow,
                downwards: downwards,
                leftwards: leftwards,
                addHeight: addHeight,
                addWidth: addWidth);
            _lastScheduledMoveEvent();

            AssertSinglePublishedWindowEventMeetsExpectations();
        }

        [Test]
        public void ShouldFireMoveEventWithCorrectDuration()
        {
            _expected.Action = WindowEvent.WindowAction.Move;
            _expected.TriggeredAt = new DateTime(2014, 7, 1, 16, 32, 10);
            _expected.Duration = TimeSpan.FromSeconds(3);
            GivenWindowIsKnown(_testWindow);

            TestDateUtils.Now = _expected.TriggeredAt.GetValueOrDefault();
            WhenWindowIsMoved(_testWindow, downwards: 27, leftwards: 0, addHeight: 0, addWidth: 0);
            TestDateUtils.Now = _expected.TerminatedAt.GetValueOrDefault();
            WhenWindowIsMoved(_testWindow, downwards: 32, leftwards: 0, addHeight: 0, addWidth: 0);
            _lastScheduledMoveEvent();

            AssertSinglePublishedWindowEventMeetsExpectations();
        }

        [Test]
        public void ShouldFireWindowClosingEvent()
        {
            _expected.Action = WindowEvent.WindowAction.Close;

            _mockWindowEvents.Raise(we => we.WindowClosing += null, _testWindow);

            AssertSinglePublishedWindowEventMeetsExpectations();
        }

        private void GivenWindowIsKnown(Window testWindow)
        {
            WhenWindowIsCreated(testWindow);
            DropAllEvents();
        }

        private void WhenWindowIsCreated(Window testWindow)
        {
            _mockWindowEvents.Raise(we => we.WindowCreated += null, testWindow);
        }

        private void WhenWindowIsActivated(Window testWindow)
        {
            _mockWindowEvents.Raise(we => we.WindowActivated += null, testWindow, null);
        }

        private void WhenWindowIsMoved(Window window, int leftwards, int downwards, int addWidth, int addHeight)
        {
            window.Top += downwards;
            window.Left += leftwards;
            window.Width += addWidth;
            window.Height += addHeight;
            _mockWindowEvents.Raise(
                we => we.WindowMoved += null,
                window,
                window.Top,
                window.Left,
                window.Width,
                window.Height);
        }

        private void AssertSinglePublishedWindowEventMeetsExpectations()
        {
            var actual = GetSinglePublished<WindowEvent>();
            Assert.AreEqual(_expected, actual);
        }
    }
}
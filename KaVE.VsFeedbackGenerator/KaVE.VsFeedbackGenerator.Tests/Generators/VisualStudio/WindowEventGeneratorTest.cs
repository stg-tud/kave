using System;
using System.Linq;
using EnvDTE;
using KaVE.Model.Events.VisualStudio;
using KaVE.Utils.DateTime;
using KaVE.VsFeedbackGenerator.Generators.VisualStudio;
using KaVE.VsFeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.VisualStudio
{
    [TestFixture]
    class WindowEventGeneratorTest : EventGeneratorTestBase
    {
        private TestIDESession _testIDESession;
        private Mock<WindowEvents> _mockWindowEvents;
        private Window _testWindow;

        [TestFixtureSetUp]
        public void SetUpTestWindow()
        {
            var mockWindow = new Mock<Window>();
            mockWindow.Setup(window => window.Type).Returns(vsWindowType.vsWindowTypeWatch);
            mockWindow.Setup(window => window.Caption).Returns("TestCaption");
            _testWindow = mockWindow.Object;
        }

        [SetUp]
        public void SetUpIDESession()
        {
            _mockWindowEvents = new Mock<WindowEvents>();
            var mockEvents = new Mock<Events>();
            // ReSharper disable once UseIndexedProperty
            mockEvents.Setup(events => events.get_WindowEvents(It.IsAny<Window>())).Returns(_mockWindowEvents.Object);
            _testIDESession = new TestIDESession();
            _testIDESession.MockDTE.Setup(dte => dte.Events).Returns(mockEvents.Object);
        }

        [Test]
        public void ShouldFireMoveEventWithing200MSAfterSingleMove()
        {
            // ReSharper disable once UnusedVariable
            var generator = new WindowEventGenerator(_testIDESession, TestMessageBus);

            var eventTime = DateTime.Now;
            WhenTestWindowIsMoved();
            WaitFor(200);

            var windowEvent = GetSinglePublishedEventAs<WindowEvent>();
            Assert.AreEqual(_testWindow.GetName(), windowEvent.Window);
            AssertAreEquals(eventTime, windowEvent.TerminatedAt.GetValueOrDefault(), 50);
        }

        [Test]
        public void ShouldFireSingleMoveEventForMultipleMovesWithin150MS()
        {
            // ReSharper disable once UnusedVariable
            var generator = new WindowEventGenerator(_testIDESession, TestMessageBus);

            WhenTestWindowIsMoved();
            WaitFor(100);
            var secondEventTime = DateTime.Now;
            WhenTestWindowIsMoved();
            WaitFor(200);

            var windowEvent = GetSinglePublishedEventAs<WindowEvent>();
            Assert.That(secondEventTime <= windowEvent.TerminatedAt);
            AssertAreEquals(secondEventTime, windowEvent.TerminatedAt.GetValueOrDefault(), 50);
        }

        [Test]
        public void ShouldFireMultipleMoveEventsIfMoreThan150MSPassBetweenEvents()
        {
            // ReSharper disable once UnusedVariable
            var generator = new WindowEventGenerator(_testIDESession, TestMessageBus);

            WhenTestWindowIsMoved();
            WaitFor(200);
            WhenTestWindowIsMoved();
            WaitFor(300);

            Assert.AreEqual(2, GetPublishedEvents().Count());
        }

        private void WhenTestWindowIsMoved()
        {
            _mockWindowEvents.Raise(we => we.WindowMoved += null, _testWindow, 0, 0, 0, 0);
        }

        private static void WaitFor(uint milliseconds)
        {
            System.Threading.Thread.Sleep((int) milliseconds);
        }

        private static void AssertAreEquals(DateTime dt1, DateTime dt2, uint tollerance)
        {
            Assert.IsTrue(new SimilarDateTimeComparer(tollerance).Equal(dt1, dt2));
        }
    }
}

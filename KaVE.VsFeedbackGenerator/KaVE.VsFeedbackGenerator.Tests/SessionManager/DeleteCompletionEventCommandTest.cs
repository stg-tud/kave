using System.Collections.Generic;
using System.Linq;
using JetBrains;
using KaVE.Model.Events;
using KaVE.Model.Events.ReSharper;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Tests.Interactivity;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class DeleteCompletionEventCommandTest
    {
        private const string TestLogFileName = "testLogFileName";

        private SessionViewModel _uut;
        private Mock<ILogFileManager<IDEEvent>> _mockLogFileManager;
        private InteractionRequestTestHelper<Confirmation> _confirmationRequestHelper;
        private List<IDEEvent> _displayedEvents;

        [SetUp]
        public void SetUp()
        {
            _displayedEvents = new List<IDEEvent> { new ActionEvent(), new BuildEvent(), new BulbActionEvent() };

            var mockReader = new Mock<ILogReader<IDEEvent>>();
            mockReader.Setup(r => r.ReadAll()).Returns(_displayedEvents);

            _mockLogFileManager = new Mock<ILogFileManager<IDEEvent>>();
            _mockLogFileManager.Setup(mgr => mgr.NewLogReader(TestLogFileName))
                               .Returns(mockReader.Object);

            _uut = new SessionViewModel(_mockLogFileManager.Object, TestLogFileName);
            _confirmationRequestHelper = _uut.ConfirmationRequest.NewTestHelper();
        }

        [Test]
        public void ShouldBeDisabledIfNoEventIsSelected()
        {
            GivenEventsAreSelected(/* none */);

            var deletionEnabled = _uut.DeleteEventsCommand.CanExecute(null);

            Assert.IsFalse(deletionEnabled);
        }

        [Test]
        public void ShouldBeEnabledIfAnEventIsSelected()
        {
            GivenEventsAreSelected(_displayedEvents[1]);

            var deletionEnabled = _uut.DeleteEventsCommand.CanExecute(null);

            Assert.IsTrue(deletionEnabled);
        }

        [Test]
        public void ShouldAskForConfirmationIfDeleteIsPressed()
        {
            GivenEventsAreSelected(_displayedEvents[0]);

            _uut.DeleteEventsCommand.Execute(null);

            Assert.IsTrue(_confirmationRequestHelper.IsRequestRaised);
        }

        [Test]
        public void ShouldAskForConfirmationForSingleSession()
        {
            GivenEventsAreSelected(_displayedEvents[0]);

            _uut.DeleteEventsCommand.Execute(null);

            var expected = new Confirmation
            {
                Caption = Messages.EventDeleteConfirmTitle,
                Message = Messages.EventDeleteConfirmSingular
            };
            var actual = _confirmationRequestHelper.Context;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldAskForConfirmationForMultipleSession()
        {
            GivenEventsAreSelected(_displayedEvents[0], _displayedEvents[1], _displayedEvents[2]);

            _uut.DeleteEventsCommand.Execute(null);

            var expected = new Confirmation
            {
                Caption = Messages.EventDeleteConfirmTitle,
                Message = Messages.EventDeleteConfirmPlural.FormatEx(3)
            };
            var actual = _confirmationRequestHelper.Context;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDoNothingIfConfirmationIsDenied()
        {
            GivenEventsAreSelected(_displayedEvents[2]);

            _uut.DeleteEventsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = false;
            _confirmationRequestHelper.Callback();

            Assert.AreEqual(3, _uut.Events.Count());
        }

        [Test]
        public void ShouldRemoveEventFromLogAndViewModelWhenConfirmationIsGiven()
        {
            var remainingEvents = new List<IDEEvent>();
            var mockWriter = new Mock<ILogWriter<IDEEvent>>();
            mockWriter.Setup(w => w.Write(It.IsAny<IDEEvent>())).Callback((IDEEvent e) => remainingEvents.Add(e));
            _mockLogFileManager.Setup(mgr => mgr.NewLogWriter(It.IsAny<string>())).Returns(mockWriter.Object);
            GivenEventsAreSelected(_displayedEvents[1]);

            _uut.DeleteEventsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            _confirmationRequestHelper.Callback();

            Assert.AreEqual(2, remainingEvents.Count);
            CollectionAssert.Contains(remainingEvents, _displayedEvents[0]);
            CollectionAssert.Contains(remainingEvents, _displayedEvents[2]);
            Assert.AreEqual(2, _uut.Events.Count());
        }

        private void GivenEventsAreSelected(params IDEEvent[] events)
        {
            _uut.SelectedEvents = _uut.Events.Where(ev => events.Contains(ev.Event));
        }
    }
}
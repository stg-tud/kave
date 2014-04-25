using System.Collections.Generic;
using System.Linq;
using JetBrains;
using KaVE.Model.Events;
using KaVE.Model.Events.ReSharper;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Tests.Interactivity;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class DeleteEventCommandTest
    {
        private SessionViewModel _uut;
        private Mock<ILog<IDEEvent>> _mockLog;
        private InteractionRequestTestHelper<Confirmation> _confirmationRequestHelper;
        private List<IDEEvent> _displayedEvents;

        [SetUp]
        public void SetUp()
        {
            _displayedEvents = new List<IDEEvent> {new ActionEvent(), new BuildEvent(), new BulbActionEvent()};

            var mockLogReader = new Mock<ILogReader<IDEEvent>>();
            mockLogReader.Setup(reader => reader.ReadAll()).Returns(_displayedEvents);

            _mockLog = new Mock<ILog<IDEEvent>>();
            _mockLog.Setup(log => log.NewLogReader()).Returns(mockLogReader.Object);

            _uut = new SessionViewModel(_mockLog.Object);
            _confirmationRequestHelper = _uut.ConfirmationRequest.NewTestHelper();
        }

        [Test]
        public void ShouldBeDisabledIfNoEventIsSelected()
        {
            GivenEventsAreSelected( /* none */);

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
            GivenEventsAreSelected(_displayedEvents[1]);

            _uut.DeleteEventsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            _confirmationRequestHelper.Callback();

            _mockLog.Verify(log => log.RemoveRange(new[] {_displayedEvents[1]}));
            Assert.AreEqual(2, _uut.Events.Count());
        }

        private void GivenEventsAreSelected(params IDEEvent[] events)
        {
            _uut.SelectedEvents = _uut.Events.Where(ev => events.Contains(ev.Event));
        }
    }
}
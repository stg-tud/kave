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
using System.Linq;
using JetBrains;
using KaVE.Model.Events;
using KaVE.TestUtils.Model.Events;
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
        private Mock<ILog> _mockLog;
        private InteractionRequestTestHelper<Confirmation> _confirmationRequestHelper;
        private List<IDEEvent> _displayedEvents;

        [SetUp]
        public void SetUp()
        {
            _displayedEvents = IDEEventTestFactory.CreateAnonymousEvents(3);

            _mockLog = new Mock<ILog>();
            _mockLog.Setup(log => log.ReadAll()).Returns(_displayedEvents);

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

        [Test]
        public void ShouldRemoveMultipleEventsFromLogAndViewModelWhenConformationIsGiven()
        {
            GivenEventsAreSelected(_displayedEvents[0], _displayedEvents[2]);

            _uut.DeleteEventsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            _confirmationRequestHelper.Callback();

            _mockLog.Verify(log => log.RemoveRange(new[] { _displayedEvents[0], _displayedEvents[2] }));
            Assert.AreEqual(1, _uut.Events.Count());
        }

        private void GivenEventsAreSelected(params IDEEvent[] events)
        {
            _uut.SelectedEvents = _uut.Events.Where(ev => events.Contains(ev.Event));
        }
    }
}
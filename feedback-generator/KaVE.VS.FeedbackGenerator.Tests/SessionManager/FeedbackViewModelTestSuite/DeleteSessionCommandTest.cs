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
using System.Linq;
using System.Threading;
using JetBrains;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager;
using KaVE.VS.FeedbackGenerator.Tests.Interactivity;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.FeedbackViewModelTestSuite
{
    internal class DeleteSessionCommandTest
    {
        private FeedbackViewModel _uut;
        private IList<Mock<ILog>> _mockLogs;
        private Mock<ILogManager> _mockLogFileManager;
        private InteractionRequestTestHelper<Confirmation> _confirmationRequestHelper;
        private IList<SessionViewModel> _sessionViewModels;

        [SetUp]
        public void SetUp()
        {
            Registry.RegisterComponent(new Mock<IIoUtils>().Object);

            _mockLogs = new List<Mock<ILog>> {LogTestHelper.MockLog(), LogTestHelper.MockLog(), LogTestHelper.MockLog()};

            _mockLogFileManager = new Mock<ILogManager>();
            _mockLogFileManager.Setup(mgr => mgr.Logs).Returns(_mockLogs.Select(m => m.Object));

            _uut = new FeedbackViewModel(_mockLogFileManager.Object);
            _uut.Refresh();
            while (_uut.IsBusy)
            {
                Thread.Sleep(5);
            }

            _confirmationRequestHelper = _uut.ConfirmationRequest.NewTestHelper();
            _sessionViewModels = _uut.Sessions.ToList();
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldBeDisabledIfNoSessionIsSelected()
        {
            _uut.SelectedSessions = new List<SessionViewModel>();

            var deletionEnabled = _uut.DeleteSessionsCommand.CanExecute(null);

            Assert.IsFalse(deletionEnabled);
        }

        [Test]
        public void ShouldBeEnabledIfASessionIsSelected()
        {
            GivenSessionsAreSelected(_sessionViewModels[0]);

            var deletionEnabled = _uut.DeleteSessionsCommand.CanExecute(null);

            Assert.IsTrue(deletionEnabled);
        }

        [Test]
        public void ShouldAskForConfirmationIfDeleteIsPressed()
        {
            GivenSessionsAreSelected(_sessionViewModels[0]);

            _uut.DeleteSessionsCommand.Execute(null);

            Assert.IsTrue(_confirmationRequestHelper.IsRequestRaised);
        }

        [Test]
        public void ShouldAskForConfirmationForSingleSession()
        {
            GivenSessionsAreSelected(_sessionViewModels[0]);

            _uut.DeleteSessionsCommand.Execute(null);

            var expected = new Confirmation
            {
                Caption = Properties.SessionManager.SessionDeleteConfirmTitle,
                Message = Properties.SessionManager.SessionDeleteConfirmSingular
            };
            var actual = _confirmationRequestHelper.Context;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldAskForConfirmationForMultipleSession()
        {
            GivenSessionsAreSelected(_sessionViewModels[0], _sessionViewModels[2]);

            _uut.DeleteSessionsCommand.Execute(null);

            var expected = new Confirmation
            {
                Caption = Properties.SessionManager.SessionDeleteConfirmTitle,
                Message = Properties.SessionManager.SessionDeleteConfirmPlural.FormatEx(2)
            };
            var actual = _confirmationRequestHelper.Context;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDoNothingIfConfirmationIsDenied()
        {
            GivenSessionsAreSelected(_sessionViewModels[0]);
            var expected = _uut.Sessions.ToList();

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = false;
            _confirmationRequestHelper.Callback();

            CollectionAssert.AreEqual(expected, _uut.Sessions);
        }

        [Test]
        public void ShouldDeleteSelectedSessionIfConfirmationIsGiven()
        {
            GivenSessionsAreSelected(_sessionViewModels[1]);

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            _confirmationRequestHelper.Callback();

            var expected = new[] {_sessionViewModels[0], _sessionViewModels[2]};
            CollectionAssert.AreEqual(expected, _uut.Sessions);
            _mockLogs[1].Verify(log => log.Delete());
        }

        [Test]
        public void ShouldDeleteMultipleSelectedSessionsIfConfirmationIsGiven()
        {
            GivenSessionsAreSelected(_sessionViewModels[0], _sessionViewModels[2]);

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            _confirmationRequestHelper.Callback();

            var expected = new[] {_sessionViewModels[1]};
            CollectionAssert.AreEqual(expected, _uut.Sessions);
            _mockLogs[0].Verify(log => log.Delete());
            _mockLogs[2].Verify(log => log.Delete());
        }

        [Test]
        public void ShouldNotRemoveSessionFromViewModelWhenDeletionOfFileFails()
        {
            _mockLogs[0].Setup(log => log.Delete()).Throws<Exception>();
            GivenSessionsAreSelected(_sessionViewModels[0]);
            var selectedSession = _uut.Sessions.First();

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            // ReSharper disable once EmptyGeneralCatchClause
            try
            {
                _confirmationRequestHelper.Callback();
            }
            catch {}

            Assert.AreEqual(selectedSession, _uut.Sessions.First());
        }

        private void GivenSessionsAreSelected(params SessionViewModel[] sessions)
        {
            _uut.SelectedSessions = sessions;
        }
    }
}
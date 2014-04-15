using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains;
using KaVE.Model.Events;
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
    internal class DeleteSessionCommandTest
    {
        private FeedbackViewModel _uut;
        private Mock<ILogFileManager<IDEEvent>> _mockLogFileManager;
        private InteractionRequestTestHelper<Confirmation> _confirmationRequestHelper;

        [SetUp]
        public void SetUp()
        {
            Registry.RegisterComponent(new Mock<IIoHelper>().Object);
            _mockLogFileManager = new Mock<ILogFileManager<IDEEvent>>();
            _mockLogFileManager.Setup(mgr => mgr.NewLogReader(It.IsAny<string>()))
                               .Returns(new Mock<ILogReader<IDEEvent>>().Object);

            _uut = new FeedbackViewModel(_mockLogFileManager.Object, null);
            _confirmationRequestHelper = _uut.ConfirmationRequest.NewTestHelper();
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
            GivenViewModelRepresents("file1");
            GivenSessionsAreSelected("file1");

            var deletionEnabled = _uut.DeleteSessionsCommand.CanExecute(null);

            Assert.IsTrue(deletionEnabled);
        }

        [Test]
        public void ShouldAskForConfirmationIfDeleteIsPressed()
        {
            GivenViewModelRepresents("file1");
            GivenSessionsAreSelected("file1");

            _uut.DeleteSessionsCommand.Execute(null);

            Assert.IsTrue(_confirmationRequestHelper.IsRequestRaised);
        }

        [Test]
        public void ShouldAskForConfirmationForSingleSession()
        {
            GivenViewModelRepresents("file1");
            GivenSessionsAreSelected("file1");

            _uut.DeleteSessionsCommand.Execute(null);

            var expected = new Confirmation
            {
                Caption = Messages.SessionDeleteConfirmTitle,
                Message = Messages.SessionDeleteConfirmSingular
            };
            var actual = _confirmationRequestHelper.Context;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldAskForConfirmationForMultipleSession()
        {
            GivenViewModelRepresents("file1", "file2", "file3");
            GivenSessionsAreSelected("file1", "file3");

            _uut.DeleteSessionsCommand.Execute(null);

            var expected = new Confirmation
            {
                Caption = Messages.SessionDeleteConfirmTitle,
                Message = Messages.SessionDeleteConfirmPlural.FormatEx(2)
            };
            var actual = _confirmationRequestHelper.Context;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDoNothingIfConfirmationIsDenied()
        {
            GivenViewModelRepresents("a.file");
            GivenSessionsAreSelected("a.file");
            var expected = _uut.Sessions.ToList();

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = false;
            _confirmationRequestHelper.Callback();

            CollectionAssert.AreEqual(expected, _uut.Sessions);
        }

        [Test]
        public void ShouldDeleteSelectedSessionIfConfirmationIsGiven()
        {
            GivenViewModelRepresents("file1", "file2");
            GivenSessionsAreSelected("file1");
            var expected = _uut.Sessions.Where(s => s.LogFileName != "file1").ToList();

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            _confirmationRequestHelper.Callback();

            CollectionAssert.AreEqual(expected, _uut.Sessions);
            _mockLogFileManager.Verify(mgr => mgr.DeleteLogs("file1"));
        }

        [Test]
        public void ShouldNotRemoveSessionFromViewModelWhenDeletionOfFileFails()
        {
            _mockLogFileManager.Setup(mgr => mgr.DeleteLogs(It.IsAny<string[]>())).Throws<Exception>();
            GivenViewModelRepresents("afile");
            GivenSessionsAreSelected("afile");

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            // ReSharper disable once EmptyGeneralCatchClause
            try {_confirmationRequestHelper.Callback();} catch {}

            Assert.AreEqual("afile", _uut.Sessions.First().LogFileName);
        }

        private void GivenViewModelRepresents(params string[] logFileNames)
        {
            _mockLogFileManager.Setup(mgr => mgr.GetLogFileNames()).Returns(logFileNames);
            RefreshViewModel();
        }

        private void GivenSessionsAreSelected(params string[] logFileNames)
        {
            _uut.SelectedSessions = _uut.Sessions.Where(s => logFileNames.Contains(s.LogFileName));
        }

        private void RefreshViewModel()
        {
            _uut.Refresh();
            while (_uut.Refreshing)
            {
                Thread.Sleep(5);
            }
        }
    }
}
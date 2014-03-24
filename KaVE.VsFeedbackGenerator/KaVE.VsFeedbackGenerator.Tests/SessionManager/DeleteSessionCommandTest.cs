using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains;
using KaVE.Model.Events;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Tests.Interactivity;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;
using Thread = System.Threading.Thread;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    class DeleteSessionCommandTest
    {
        private FeedbackViewModel _uut;
        private Mock<ILogFileManager<IDEEvent>> _mockLogFileManager;
        private InteractionRequestTestHelper<Confirmation> _confirmationRequestHelper;

        [SetUp]
        public void SetUp()
        {
            _mockLogFileManager = new Mock<ILogFileManager<IDEEvent>>();
            _mockLogFileManager.Setup(mgr => mgr.NewLogReader(It.IsAny<string>()))
                               .Returns(new Mock<ILogReader<IDEEvent>>().Object);

            _uut = new FeedbackViewModel(_mockLogFileManager.Object, null);
            _confirmationRequestHelper = _uut.ConfirmationRequest.NewTestHelper();
        }

        [Test]
        public void ShouldBeDisabledIfNoSessionIsSelected()
        {
            _uut.SelectedSessions = new List<SessionView>();

            var deletionEnabled = _uut.DeleteSessionsCommand.CanExecute(null);

            Assert.IsFalse(deletionEnabled);
        }

        [Test]
        public void ShouldBeEnabledIfASessionIsSelected()
        {
            GivenViewModelRepresents("file1", "file2");
            GivenSelectionAt(0);

            var deletionEnabled = _uut.DeleteSessionsCommand.CanExecute(null);

            Assert.IsTrue(deletionEnabled);
        }

        [Test]
        public void ShouldAskForConfirmationIfDeleteIsPressed()
        {
            GivenViewModelRepresents("file1");
            GivenSelectionAt(0);

            _uut.DeleteSessionsCommand.Execute(null);

            Assert.IsTrue(_confirmationRequestHelper.IsRequestRaised);
        }

        [Test]
        public void ShouldAskForConfirmationForSingleSession()
        {
            GivenViewModelRepresents("file1");
            GivenSelectionAt(0);

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
            GivenSelectionAt(0,2);

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
            GivenSelectionAt(0);
            var expected = _uut.Sessions.ToList();

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = false;
            _confirmationRequestHelper.Callback();

            CollectionAssert.AreEqual(expected, _uut.Sessions);
        }

        // TODO encapsulate file deletion in the log file manager and only check for call on the mock here
        [Test]
        public void ShouldDeleteSelectedSessionIfConfirmationIsGiven()
        {
            var logFile1Name = IoTestHelper.GetTempFileName();
            GivenViewModelRepresents(logFile1Name, "file2");
            GivenSelectionAt(0);
            var expected = _uut.Sessions.Skip(1).ToList();

            _uut.DeleteSessionsCommand.Execute(null);
            _confirmationRequestHelper.Context.Confirmed = true;
            _confirmationRequestHelper.Callback();

            CollectionAssert.AreEqual(expected, _uut.Sessions);
            Assert.IsFalse(File.Exists(logFile1Name));
        }

        // TODO test that session remains in UI when log-file manager fails to delete

        private void GivenViewModelRepresents(params string[] logFileNames)
        {
            _mockLogFileManager.Setup(mgr => mgr.GetLogFileNames()).Returns(logFileNames);
            RefreshViewModel();
        }

        private void GivenSelectionAt(params int[] indexes)
        {
            var enumerator = _uut.Sessions.GetEnumerator();
            var i = 0;
            var selectedSessions = new List<SessionView>();
            while (enumerator.MoveNext())
            {
                if (indexes.Contains(i))
                {
                    selectedSessions.Add(enumerator.Current);
                }
                i++;
            }

            _uut.SelectedSessions = selectedSessions;
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

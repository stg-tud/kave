using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.Model.Events;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
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
        private InteractionRequestTestHelper<Confirmation> _deleteSessionConfirmationRequestHelper;

        [SetUp]
        public void SetUp()
        {
            _mockLogFileManager = new Mock<ILogFileManager<IDEEvent>>();
            _mockLogFileManager.Setup(mgr => mgr.NewLogReader(It.IsAny<string>()))
                               .Returns(new Mock<ILogReader<IDEEvent>>().Object);

            _uut = new FeedbackViewModel(_mockLogFileManager.Object, null);

            _deleteSessionConfirmationRequestHelper = new InteractionRequestTestHelper<Confirmation>(_uut.DeleteSessionsConfirmationRequest);
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
            GivenSessionManagerDisplaysSessions("file1", "file2");
            GivenSelectionAt(0);

            var deletionEnabled = _uut.DeleteSessionsCommand.CanExecute(null);

            Assert.IsTrue(deletionEnabled);
        }

        [Test]
        public void ShouldAskForConfirmationIfDeleteIsPressed()
        {
            GivenSessionManagerDisplaysSessions("file1");
            GivenSelectionAt(0);

            _uut.DeleteSessionsCommand.Execute(null);

            Assert.IsTrue(_deleteSessionConfirmationRequestHelper.IsRequestRaised);
        }

        // TODO write tests for other message (parts)
        [Test]
        public void ShouldAskForConfirmation_CheckTitle()
        {
            GivenSessionManagerDisplaysSessions("file1");
            GivenSelectionAt(0);

            _uut.DeleteSessionsCommand.Execute(null);

            var expected = Messages.SessionDeleteConfirmTitle;
            var actual = _deleteSessionConfirmationRequestHelper.Context.Title;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDoNothingIfConfirmationIsDenied()
        {
            GivenSessionManagerDisplaysSessions("a.file");
            GivenSelectionAt(0);
            var expected = _uut.Sessions.ToList();

            _uut.DeleteSessionsCommand.Execute(null);
            _deleteSessionConfirmationRequestHelper.Context.Confirmed = false;
            _deleteSessionConfirmationRequestHelper.Callback();

            CollectionAssert.AreEqual(expected, _uut.Sessions);
        }

        // TODO encapsulate file deletion in the log file manager and only check for call on the mock here
        [Test]
        public void ShouldDeleteSelectedSessionIfConfirmationIsGiven()
        {
            var logFile1Name = IoTestHelper.GetTempFileName();
            GivenSessionManagerDisplaysSessions(logFile1Name, "file2");
            GivenSelectionAt(0);
            var expected = _uut.Sessions.Skip(1).ToList();

            _uut.DeleteSessionsCommand.Execute(null);
            _deleteSessionConfirmationRequestHelper.Context.Confirmed = true;
            _deleteSessionConfirmationRequestHelper.Callback();

            CollectionAssert.AreEqual(expected, _uut.Sessions);
            Assert.IsFalse(File.Exists(logFile1Name));
        }

        // TODO test that session remains in UI when log-file manager fails to delete

        private void GivenSessionManagerDisplaysSessions(params string[] logFileNames)
        {
            _mockLogFileManager.Setup(mgr => mgr.GetLogFileNames()).Returns(logFileNames);
            RefreshViewAndWait();
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

        private void RefreshViewAndWait()
        {
            _uut.Refresh();
            while (_uut.Refreshing)
            {
                Thread.Sleep(5);
            }
        }
    }

    public class InteractionRequestTestHelper<T> where T : Notification
    {
        public bool IsRequestRaised { get; private set; }
        public string Title { get; private set; }
        public T Context { get; private set; }
        public Action Callback { get; private set; } 

        public InteractionRequestTestHelper(IInteractionRequest request)
        {
            request.Raised += (s, e) =>
            {
                IsRequestRaised = true;
                Title = e.Context.Title;
                Context = (T) e.Context;
                Callback = e.Callback;
            };
        }
    }
}

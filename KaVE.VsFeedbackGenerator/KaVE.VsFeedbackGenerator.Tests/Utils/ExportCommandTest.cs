using System;
using System.Collections.Generic;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class ExportCommandTest
    {
        private Mock<ISessionExport> _sessionExportMock;
        private Mock<Func<string, ILogWriter<IDEEvent>>> _logWriterMock;
        private Mock<IFeedbackViewModelDialog> _feedbackViewModelDialog;

        [SetUp]
        public void SetUp()
        {
            _sessionExportMock = new Mock<ISessionExport>();
            _logWriterMock = new Mock<Func<string, ILogWriter<IDEEvent>>>();
            _feedbackViewModelDialog = new Mock<IFeedbackViewModelDialog>();
        }

        private DelegateCommand CreateSut()
        {
            var uut = ExportCommand.Create(
                _sessionExportMock.Object,
                _logWriterMock.Object,
                _feedbackViewModelDialog.Object);
            return uut;
        }

        [TestCase(true), TestCase(false)]
        public void ShouldForwardCanExecute(bool areAnyEventsPresent)
        {
            _feedbackViewModelDialog.Setup(d => d.AreAnyEventsPresent).Returns(areAnyEventsPresent);

            var uut = CreateSut();

            Assert.AreEqual(areAnyEventsPresent, uut.CanExecute(null));
        }

        // TODO split into smaller test-methods?
        [Test]
        public void ShouldShowDialogOnSuccess()
        {
            const int eventCount = 25;
            var events = CreateAnonymousEvents(eventCount);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            var uut = CreateSut();
            uut.Execute(null);

            _sessionExportMock.Verify(s => s.Export(events, _logWriterMock.Object));
            _feedbackViewModelDialog.Verify(d => d.ShowExportSucceededMessage(eventCount));
        }

        [Test]
        public void ShouldShowDialogOnFailure()
        {
            var events = CreateAnonymousEvents(5);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            const string failMessage = "custom message";
            _sessionExportMock.Setup(s => s.Export(events, _logWriterMock.Object)).Throws(new AssertException(failMessage));

            var uut = CreateSut();
            uut.Execute(null);

            _feedbackViewModelDialog.Verify(d => d.ShowExportFailedMessage(failMessage));
        }

        private static List<IDEEvent> CreateAnonymousEvents(int num)
        {
            var list = new List<IDEEvent>();
            for (var i = 0; i < num; i ++)
            {
                list.Add(new Mock<IDEEvent>().Object);
            }
            return list;
        }
    }
}
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
    internal class ExportCommandFactoryTest
    {
        private const string ArbitraryFileName = "xyz";
        private const string ArbitrayExceptionMessage = "custom message";

        private Mock<ILogFileManager<IDEEvent>> _logFileManagerMock;
        private Mock<ILogWriter<IDEEvent>> _logWriterMock;
        private Mock<IFeedbackViewModelDialog> _feedbackViewModelDialog;
        private Mock<IPublisher> _publisherMock;

        private ExportCommandFactory _factory;
        private Mock<IIoUtils> _ioHelperMock;

        [SetUp]
        public void SetUp()
        {
            _logFileManagerMock = new Mock<ILogFileManager<IDEEvent>>();
            _logWriterMock = new Mock<ILogWriter<IDEEvent>>();
            _feedbackViewModelDialog = new Mock<IFeedbackViewModelDialog>();
            _publisherMock = new Mock<IPublisher>();

            _ioHelperMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(_ioHelperMock.Object);

            _logFileManagerMock.Setup(m => m.NewLogWriter(It.IsAny<string>())).Returns(_logWriterMock.Object);

            _factory = new ExportCommandFactory(_logFileManagerMock.Object, _feedbackViewModelDialog.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        public DelegateCommand CreateSut()
        {
            return _factory.Create(_publisherMock.Object);
        }

        [TestCase(true), TestCase(false)]
        public void ShouldForwardCanExecute(bool areAnyEventsPresent)
        {
            _feedbackViewModelDialog.Setup(d => d.AreAnyEventsPresent).Returns(areAnyEventsPresent);
            Assert.AreEqual(areAnyEventsPresent, CreateSut().CanExecute(null));
        }

        [Test]
        public void ShouldInvokeWriter()
        {
            var events = CreateAnonymousEvents(25);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            _ioHelperMock.Setup(io => io.GetTempFileName()).Returns(ArbitraryFileName);

            var uut = CreateSut();
            uut.Execute(null);

            _logWriterMock.Verify(w => w.WriteAll(events));
        }

        [Test]
        public void ShouldInvokePublisher()
        {
            var events = CreateAnonymousEvents(25);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            _ioHelperMock.Setup(io => io.GetTempFileName()).Returns(ArbitraryFileName);

            var uut = CreateSut();
            uut.Execute(null);

            _publisherMock.Verify(p => p.Publish(ArbitraryFileName));
        }

        [Test]
        public void ShouldInvokeFileManager()
        {
            var events = CreateAnonymousEvents(25);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            _ioHelperMock.Setup(io => io.GetTempFileName()).Returns(ArbitraryFileName);

            var uut = CreateSut();
            uut.Execute(null);

            _logFileManagerMock.Verify(p => p.NewLogWriter(ArbitraryFileName));
        }

        [Test]
        public void ShouldShowDialogOnSuccess()
        {
            const int eventCount = 25;
            var events = CreateAnonymousEvents(eventCount);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            var uut = CreateSut();
            uut.Execute(null);

            _feedbackViewModelDialog.Verify(d => d.ShowExportSucceededMessage(eventCount));
        }

        [Test]
        public void ShouldShowDialogOnPublishingFailure()
        {
            var events = CreateAnonymousEvents(5);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            SetupFailingPublisherMock(ArbitrayExceptionMessage);

            var uut = CreateSut();
            uut.Execute(null);

            _feedbackViewModelDialog.Verify(d => d.ShowExportFailedMessage(ArbitrayExceptionMessage));
        }

        [Test]
        public void ShouldShowDialogOnLogWritingFailure()
        {
            var events = CreateAnonymousEvents(5);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            SetupFailingLogWriterMock(ArbitrayExceptionMessage);

            var uut = CreateSut();
            uut.Execute(null);

            _feedbackViewModelDialog.Verify(d => d.ShowExportFailedMessage(ArbitrayExceptionMessage));
        }

        private void SetupFailingPublisherMock(string failMessage)
        {
            _publisherMock.Setup(s => s.Publish(It.IsAny<string>()))
                          .Throws(new AssertException(failMessage));
        }

        private void SetupFailingLogWriterMock(string failMessage)
        {
            _logWriterMock.Setup(w => w.WriteAll(It.IsAny<IList<IDEEvent>>())).Throws(new AssertException(failMessage));
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
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
using System.IO;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.TestUtils.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class ExportCommandFactoryTest
    {
        private const string ArbitraryFileName = "xyz";
        private const string ArbitrayExceptionMessage = "custom message";

        private Mock<IFeedbackViewModelDialog> _feedbackViewModelDialog;
        private Mock<IPublisher> _publisherMock;

        private ExportCommandFactory _factory;
        private Mock<IIoUtils> _ioHelperMock;
        private MemoryStream _tmpExportFileStream;

        [SetUp]
        public void SetUp()
        {
            _feedbackViewModelDialog = new Mock<IFeedbackViewModelDialog>();
            _publisherMock = new Mock<IPublisher>();

            _tmpExportFileStream = new MemoryStream();
            _ioHelperMock = new Mock<IIoUtils>();
            _ioHelperMock.Setup(iou => iou.OpenFile(It.IsAny<string>(), FileMode.Open, FileAccess.Write))
                         .Returns(_tmpExportFileStream);
            Registry.RegisterComponent(_ioHelperMock.Object);

            _factory = new ExportCommandFactory(_feedbackViewModelDialog.Object);
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
        public void ShouldWriteEventsToTempFile()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(25);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            _ioHelperMock.Setup(io => io.GetTempFileName()).Returns(ArbitraryFileName);

            var uut = CreateSut();
            uut.Execute(null);

            JsonLogAssert_StreamContainsEntries(_tmpExportFileStream, events);
        }

        private static void JsonLogAssert_StreamContainsEntries(MemoryStream stream, IEnumerable<IDEEvent> expected)
        {
            JsonLogAssert_StreamContainsEntries((Stream)new MemoryStream(stream.ToArray()), expected);
        }

        private static void JsonLogAssert_StreamContainsEntries(Stream stream, IEnumerable<IDEEvent> expected)
        {
            using (var reader = new JsonLogReader<IDEEvent>(stream))
            {
                var actual = reader.ReadAll();
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void ShouldInvokePublisher()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(25);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            _ioHelperMock.Setup(io => io.GetTempFileName()).Returns(ArbitraryFileName);

            var uut = CreateSut();
            uut.Execute(null);

            _publisherMock.Verify(p => p.Publish(ArbitraryFileName));
        }

        [Test]
        public void ShouldShowDialogOnSuccess()
        {
            const int eventCount = 25;
            var events = IDEEventTestFactory.CreateAnonymousEvents(eventCount);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            var uut = CreateSut();
            uut.Execute(null);

            _feedbackViewModelDialog.Verify(d => d.ShowExportSucceededMessage(eventCount));
        }

        [Test]
        public void ShouldShowDialogOnPublishingFailure()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(5);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            SetupFailingPublisherMock(ArbitrayExceptionMessage);

            var uut = CreateSut();
            uut.Execute(null);

            _feedbackViewModelDialog.Verify(d => d.ShowExportFailedMessage(ArbitrayExceptionMessage));
        }

        [Test]
        public void ShouldShowDialogOnLogWritingFailure()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(5);
            _feedbackViewModelDialog.Setup(d => d.ExtractEventsForExport()).Returns(events);

            SetupFailingIO(ArbitrayExceptionMessage);

            var uut = CreateSut();
            uut.Execute(null);

            _feedbackViewModelDialog.Verify(d => d.ShowExportFailedMessage(ArbitrayExceptionMessage));
        }

        private void SetupFailingPublisherMock(string failMessage)
        {
            _publisherMock.Setup(s => s.Publish(It.IsAny<string>()))
                          .Throws(new AssertException(failMessage));
        }

        private void SetupFailingIO(string failMessage)
        {
            _ioHelperMock.Setup(iou => iou.OpenFile(It.IsAny<string>(), FileMode.Open, FileAccess.Write))
                         .Throws(new Exception(failMessage));
        }
    }
}
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
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using JetBrains;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Json;
using KaVE.Commons.Utils.Streams;
using KaVE.VsFeedbackGenerator.Export;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Export
{
    [TestFixture]
    internal class ExporterTest
    {
        private readonly IList<IDEEvent> _eventsForRealLifeExample = new List<IDEEvent>
        {
            new WindowEvent {TriggeredAt = new DateTime(2014, 1, 1)},
            new WindowEvent {TriggeredAt = new DateTime(2014, 1, 2)}
        };

        private Mock<IPublisher> _publisherMock;
        private Exporter _sut;
        private MemoryStream _lastPublishedStream;
        private Mock<IDataExportAnonymizer> _anonymizerMock;

        [SetUp]
        public void SetUp()
        {
            _publisherMock = new Mock<IPublisher>();
            _publisherMock.Setup(p => p.Publish(It.IsAny<MemoryStream>())).Callback<MemoryStream>(
                stream => { _lastPublishedStream = new MemoryStream(stream.ToArray()); });

            _anonymizerMock = new Mock<IDataExportAnonymizer>();
            _anonymizerMock.Setup(a => a.Anonymize(It.IsAny<IDEEvent>())).Returns<IDEEvent>(ideEvent => ideEvent);

            _sut = new Exporter(_anonymizerMock.Object);
        }

        [Test]
        public void ShouldSilentlySkipPublishingOfEmptyEventList()
        {
            _sut.Export(new IDEEvent[0], _publisherMock.Object);
            _publisherMock.Verify(p => p.Publish(It.IsAny<MemoryStream>()), Times.Never);
        }

        [Test]
        public void ShouldInvokePublisher()
        {
            var events = IDEEventTestFactory.SomeEvents(25);
            _sut.Export(events, _publisherMock.Object);
            _publisherMock.Verify(p => p.Publish(It.IsAny<MemoryStream>()));
        }

        [Test]
        public void ShouldCreateOneFilePerEvent()
        {
            var events = IDEEventTestFactory.SomeEvents(25);
            _sut.Export(events, _publisherMock.Object);
            var zipFile = GetZipFileFromExport();
            Assert.AreEqual(25, zipFile.Entries.Count);
        }

        [Test]
        public void ShouldPersistAllProvidedEvents()
        {
            var expecteds = IDEEventTestFactory.SomeEvents(25);

            _sut.Export(expecteds, _publisherMock.Object);
            var actuals = GetExportedEvents();

            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test]
        public void ShouldCreateFileNamesFromEventIndexAndType()
        {
            var events = new IDEEvent[] {new EditEvent(), new ErrorEvent(), new WindowEvent()};
            var expected = new[] {"0-EditEvent.json", "1-ErrorEvent.json", "2-WindowEvent.json"};

            _sut.Export(events, _publisherMock.Object);

            var zipFile = GetZipFileFromExport();
            var actual = zipFile.EntryFileNames;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldInvokeAnonymizerOnEveryEvent()
        {
            var expected = IDEEventTestFactory.SomeEvents(13);
            var actual = new List<IDEEvent>();
            _anonymizerMock.Setup(a => a.Anonymize(It.IsAny<IDEEvent>()))
                           .Callback<IDEEvent>(actual.Add)
                           .Returns<IDEEvent>(ideEvent => ideEvent);

            _sut.Export(expected, _publisherMock.Object);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldCreateZipFromAnonymizedEvents()
        {
            var original = IDEEventTestFactory.SomeEvents(3);
            var anonymousEvent = new TestIDEEvent {TestProperty = "It's me!"};
            _anonymizerMock.Setup(a => a.Anonymize(It.IsAny<IDEEvent>())).Returns(anonymousEvent);
            var expected = new[] {anonymousEvent, anonymousEvent, anonymousEvent};

            _sut.Export(original, _publisherMock.Object);

            var actual = GetExportedEvents();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldReportProgress()
        {
            var events = IDEEventTestFactory.SomeEvents(25);
            var expected = events.Select((e, i) => Properties.UploadWizard.WritingEvents.FormatEx(i*4)).ToList();
            expected.Add(Properties.UploadWizard.WritingEvents.FormatEx(100));
            expected.Add(Properties.UploadWizard.CompressingEvents);
            expected.Add(Properties.UploadWizard.PublishingEvents);

            var actual = new List<string>();

            Action<string> eventProcessed = actual.Add;
            _sut.StatusChanged += eventProcessed;
            _sut.Export(events, _publisherMock.Object);
            _sut.StatusChanged -= eventProcessed;

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Entry limit for standard ZIP file should not limit export")]
        public void ShouldExportMoreThan65535Events()
        {
            var manyEvents = IDEEventTestFactory.SomeEvents(65536);

            _sut.Export(manyEvents, _publisherMock.Object);
        }

        [Test, Ignore("manual test that the server receives valid file - RealLifeExample part 1")]
        public void ShouldUploadValidZipToServer()
        {
            Registry.RegisterComponent<IIoUtils>(new IoUtils());
            _sut.Export(
                _eventsForRealLifeExample,
                new HttpPublisher(new Uri("http://kave.st.informatik.tu-darmstadt.de:667/upload")));
            Registry.Clear();
        }

        [Test, Ignore("manual test that the server received valid file - RealLifeExample part 2")]
        public void ShouldCompareUploadedFile()
        {
            var zipFile = new ZipFile(@"C:\compare.zip");
            var actual = zipFile.Entries.Select(ExtractIDEEvent);
            CollectionAssert.AreEquivalent(_eventsForRealLifeExample, actual);
        }

        private IEnumerable<IDEEvent> GetExportedEvents()
        {
            var zipFile = GetZipFileFromExport();
            return zipFile.Entries.Select(ExtractIDEEvent);
        }

        private ZipFile GetZipFileFromExport()
        {
            var tempFileName = Path.GetTempFileName();
            using (var fileStream = File.Open(tempFileName, FileMode.Create))
            {
                _lastPublishedStream.CopyTo(fileStream);
            }
            return new ZipFile(tempFileName);
        }

        private static IDEEvent ExtractIDEEvent(ZipEntry entry)
        {
            using (var out2 = new MemoryStream())
            {
                entry.Extract(out2);
                var json = out2.AsString();
                return json.ParseJsonTo<IDEEvent>();
            }
        }
    }
}
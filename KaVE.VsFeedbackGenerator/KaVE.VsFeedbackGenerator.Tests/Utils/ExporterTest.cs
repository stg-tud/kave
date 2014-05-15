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
using KaVE.Model.Events;
using KaVE.Model.Events.VisualStudio;
using KaVE.TestUtils.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Tests.Utils.Json;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class ExporterTest
    {
        private Mock<IPublisher> _publisherMock;
        private Exporter _sut;
        private MemoryStream _lastPublishedStream;

        [SetUp]
        public void SetUp()
        {
            _publisherMock = new Mock<IPublisher>();
            _publisherMock.Setup(p => p.Publish(It.IsAny<MemoryStream>())).Callback<MemoryStream>(
                stream => { _lastPublishedStream = new MemoryStream(stream.ToArray()); });

            _sut = new Exporter();
        }

        [Test, Ignore("multiple enumerations on (lazy) enumerable"),
         ExpectedException(typeof (AssertException), ExpectedMessage = "no events")]
        public void ShouldFailWithNoEvents()
        {
            _sut.Export(new List<IDEEvent>(), _publisherMock.Object);
        }

        [Test]
        public void ShouldInvokePublisher()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(25);
            _sut.Export(events, _publisherMock.Object);
            _publisherMock.Verify(p => p.Publish(It.IsAny<MemoryStream>()));
        }

        [Test]
        public void ShouldCreateOneFilePerEvent()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(25);
            _sut.Export(events, _publisherMock.Object);
            var zipFile = GetZipFileFromExport();
            Assert.AreEqual(25, zipFile.Entries.Count);
        }

        [Test]
        public void ShouldPersistAllProvidedEvents()
        {
            var expecteds = IDEEventTestFactory.CreateAnonymousEvents(25);
            _sut.Export(expecteds, _publisherMock.Object);
            var zipFile = GetZipFileFromExport();

            var actuals = zipFile.Entries.Select(ExtractIDEEvent);

            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test]
        public void ShouldIncludeEventTypeInFileName()
        {
            var expected = new WindowEvent();
            _sut.Export(new List<IDEEvent> {expected}, _publisherMock.Object);

            var zipFile = GetZipFileFromExport();
            var entry = zipFile["0-WindowEvent.json"];
            Assert.NotNull(entry);

            var actual = ExtractIDEEvent(entry);
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore("manual test that the server receives valid file - RealLifeExample part 1")]
        public void ShouldUploadValidZipToServer()
        {
            Registry.RegisterComponent(new IoUtils() as IIoUtils);
            _sut.Export(
                EventsForRealLifeExample,
                new HttpPublisher(new Uri("http://kave.st.informatik.tu-darmstadt.de:667/upload")));
            Registry.Clear();
        }

        [Test, Ignore("manual test that the server received valid file - RealLifeExample part 2")]
        public void ShouldCompareUploadedFile()
        {
            var zipFile = new ZipFile(@"C:\compare.zip");
            var actual = zipFile.Entries.Select(ExtractIDEEvent);
            CollectionAssert.AreEquivalent(EventsForRealLifeExample, actual);
        }

        private IEnumerable<IDEEvent> EventsForRealLifeExample
        {
            get
            {
                return new List<IDEEvent>
                {
                    new WindowEvent {TriggeredAt = new DateTime(2014, 1, 1)},
                    new WindowEvent {TriggeredAt = new DateTime(2014, 1, 2)}
                };
            }
        }

        private ZipFile GetZipFileFromExport()
        {
            var tempFileName = Path.GetTempFileName();
            using (var fileStream = File.Open(tempFileName, FileMode.Create))
            {
                _lastPublishedStream.CopyTo(fileStream);
            }
            var zipFile = new ZipFile(tempFileName);
            return zipFile;
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
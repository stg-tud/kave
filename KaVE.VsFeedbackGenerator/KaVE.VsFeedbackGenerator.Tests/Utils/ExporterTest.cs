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
 */

using System.Collections.Generic;
using System.IO;
using KaVE.Model.Events;
using KaVE.TestUtils.Model.Events;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class ExporterTest
    {
        private const string ArbitraryFileName = "file";
        private Mock<IIoUtils> _ioUtilsMock;
        private Mock<IPublisher> _publisherMock;
        private Exporter _sut;
        private MemoryStream _memoryStream;

        [SetUp]
        public void SetUp()
        {
            _ioUtilsMock = new Mock<IIoUtils>();
            _ioUtilsMock.Setup(io => io.GetTempFileName()).Returns(ArbitraryFileName);
            _memoryStream = new MemoryStream();
            _ioUtilsMock.Setup(io => io.OpenFile(ArbitraryFileName, FileMode.Open, FileAccess.Write)).Returns(_memoryStream);
            Registry.RegisterComponent(_ioUtilsMock.Object);
            _publisherMock = new Mock<IPublisher>();
            _sut = new Exporter();
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        [Test, Ignore("multiple enumerations on (lazy) enumerable"), ExpectedException(typeof(AssertException), ExpectedMessage = "no events")]
        public void ShouldFailWithNoEvents()
        {
            _sut.Export(new List<IDEEvent>(), _publisherMock.Object);
        }

        [Test]
        public void ShouldInvokePublisher()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(25);

            _sut.Export(events, _publisherMock.Object);

            _publisherMock.Verify(p => p.Publish(ArbitraryFileName));
        }

        [Test]
        public void ShouldWriteEventsToTempFile()
        {
            var events = IDEEventTestFactory.CreateAnonymousEvents(25);

            _sut.Export(events, _publisherMock.Object);

            JsonLogAssert_StreamContainsEntries(_memoryStream, events);
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
    }
}

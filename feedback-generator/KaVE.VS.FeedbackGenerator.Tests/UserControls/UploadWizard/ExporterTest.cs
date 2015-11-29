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
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;
using KaVE.Commons.Utils.Streams;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Generators;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;
using KaVE.VS.FeedbackGenerator.Tests.Utils.Logging;
using KaVE.VS.FeedbackGenerator.Utils.Export;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UploadWizard
{
    internal class ExporterTest
    {
        private readonly IList<IDEEvent> _eventsForRealLifeExample = new List<IDEEvent>
        {
            new WindowEvent {TriggeredAt = new DateTime(2014, 1, 1)},
            new WindowEvent {TriggeredAt = new DateTime(2014, 1, 2)}
        };

        private IPublisher _publisher;
        private Exporter _sut;
        private IList<IDEEvent> _publishedEvents;
        private IDataExportAnonymizer _anonymizer;
        private IUserProfileEventGenerator _userProfileEventGenerator;
        private UserProfileEvent _userProfileEvent;
        private InMemoryLogManager _logManager;
        private static readonly DateTime SomeDate = DateTime.Today;

        [SetUp]
        public void SetUp()
        {
            _logManager = new InMemoryLogManager();

            _publishedEvents = null;
            _publisher = Mock.Of<IPublisher>();
            Mock.Get(_publisher)
                .Setup(
                    p => p.Publish(It.IsAny<UserProfileEvent>(), It.IsAny<IEnumerable<IDEEvent>>(), It.IsAny<Action>()))
                .Callback<UserProfileEvent, IEnumerable<IDEEvent>, Action>(
                    (upe, events, callback) =>
                    {
                        _publishedEvents = new List<IDEEvent>();
                        // TODO: maybe make this a bit nicer with the prepend function
                        if (upe != null)
                        {
                            _publishedEvents.Add(upe);
                        }
                        foreach (var e in events)
                        {
                            _publishedEvents.Add(e);
                            callback();
                        }
                    });

            _anonymizer = Mock.Of<IDataExportAnonymizer>();
            Mock.Get(_anonymizer).Setup(a => a.Anonymize(It.IsAny<IDEEvent>())).Returns<IDEEvent>(ideEvent => ideEvent);

            _userProfileEvent = new UserProfileEvent {ProfileId = "p"};

            _userProfileEventGenerator = Mock.Of<IUserProfileEventGenerator>();
            Mock.Get(_userProfileEventGenerator).Setup(e => e.ShouldCreateEvent()).Returns(false);
            Mock.Get(_userProfileEventGenerator).Setup(e => e.CreateEvent()).Returns(_userProfileEvent);

            _sut = new Exporter(_logManager, _anonymizer, _userProfileEventGenerator);
        }

        [Test]
        public void SkipsPublishingIfNoEvents()
        {
            WhenEverythingIsExported();

            Assert.IsNull(_publishedEvents);
        }

        [Test]
        public void PublishesEvents()
        {
            var log1Entries = TestEventFactory.SomeEvents(25);
            _logManager.Add(SomeDate, log1Entries);
            var log2Entries = TestEventFactory.SomeEvents(5);
            _logManager.Add(SomeDate.AddDays(-1), log2Entries);

            var numberOfEvents = WhenEverythingIsExported();

            AssertPublishedEvents(log1Entries.Union(log2Entries));
            Assert.AreEqual(30, numberOfEvents);
        }

        [Test]
        public void PublishesOnlyOlderEvents()
        {
            var dateTime = DateTime.Now;
            var newerEvent = TestEventFactory.SomeEvent(dateTime);
            var olderEvent = TestEventFactory.SomeEvent(dateTime.AddMinutes(-2));
            _logManager.Add(SomeDate, newerEvent, olderEvent);

            var numberOfEvents = _sut.Export(dateTime.AddMinutes(-1), _publisher);

            AssertPublishedEvents(olderEvent);
            Assert.AreEqual(1, numberOfEvents);
        }

        [Test]
        public void PublishesProfileEvent()
        {
            Mock.Get(_userProfileEventGenerator).Setup(g => g.ShouldCreateEvent()).Returns(true);
            var logEntries = TestEventFactory.SomeEvents(25);
            _logManager.Add(SomeDate, logEntries);

            var numberOfEvents = WhenEverythingIsExported();

            AssertPublishedEvents(new[] {_userProfileEvent}.Union(logEntries));
            Assert.AreEqual(26, numberOfEvents);
        }

        // TODO: Is this useful?
        [Test, Ignore]
        public void PublishesProfileEventIfNoEvents()
        {
            Mock.Get(_userProfileEventGenerator).Setup(g => g.ShouldCreateEvent()).Returns(true);

            var numberOfEvents = WhenEverythingIsExported();

            AssertPublishedEvents(_userProfileEvent);
            Assert.AreEqual(1, numberOfEvents);
        }

        [Test]
        public void AppendsProfileEvent()
        {
            Mock.Get(_userProfileEventGenerator).Setup(g => g.ShouldCreateEvent()).Returns(true);
            _logManager.Add(SomeDate, TestEventFactory.SomeEvents(3));

            WhenEverythingIsExported();

            CollectionAssert.Contains(_publishedEvents, _userProfileEvent);
        }

        [Test]
        public void InvokesAnonymizerOnEveryEvent()
        {
            var expected = TestEventFactory.SomeEvents(13);
            _logManager.Add(SomeDate, expected);
            var actual = new List<IDEEvent>();
            Mock.Get(_anonymizer).Setup(a => a.Anonymize(It.IsAny<IDEEvent>()))
                .Callback<IDEEvent>(actual.Add)
                .Returns<IDEEvent>(ideEvent => ideEvent);

            WhenEverythingIsExported();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExportsAnonymizedEvents()
        {
            _logManager.Add(SomeDate, TestEventFactory.SomeEvents(3));
            IDEEvent anonymousEvent = TestEventFactory.SomeEvent();
            Mock.Get(_anonymizer).Setup(a => a.Anonymize(It.IsAny<IDEEvent>())).Returns(anonymousEvent);

            WhenEverythingIsExported();

            AssertPublishedEvents(anonymousEvent, anonymousEvent, anonymousEvent);
        }

        [Test]
        public void ExportsErrorEventIfAnonymizerFails()
        {
            var original = TestEventFactory.SomeEvent();
            _logManager.Add(SomeDate, original);

            Mock.Get(_anonymizer).Setup(a => a.Anonymize(It.IsAny<IDEEvent>())).Returns<IDEEvent>(
                e =>
                {
                    if (!(e is ErrorEvent))
                    {
                        throw new NullReferenceException();
                    }
                    return e;
                });

            WhenEverythingIsExported();

            var actual = _publishedEvents.Single();

            Assert.IsInstanceOf<ErrorEvent>(actual);
            Assert.AreEqual("An error occured during anonymization of KaVE.Commons.TestUtils.Model.Events.TestIDEEvent.", ((ErrorEvent)actual).Content);
            Assert.NotNull(((ErrorEvent)actual).StackTrace);
            Assert.AreEqual(original.TriggeredAt, actual.TriggeredAt);
            Assert.AreEqual(original.TerminatedAt, actual.TerminatedAt);
            Assert.AreEqual(original.Duration, actual.Duration);
            Assert.AreEqual(original.IDESessionUUID, actual.IDESessionUUID);
            Assert.AreEqual(original.ActiveDocument, actual.ActiveDocument);
            Assert.AreEqual(original.ActiveWindow, actual.ActiveWindow);
            Assert.AreEqual(original.Id, actual.Id);
            Assert.AreEqual(original.KaVEVersion, actual.KaVEVersion);
            Assert.AreEqual(original.TriggeredBy, actual.TriggeredBy);
        }

        [Test]
        public void ExportsFatalErrorEventIfAnonymizerFailsTwice()
        {
            var original = TestEventFactory.SomeEvent();
            _logManager.Add(SomeDate, original);

            Mock.Get(_anonymizer).Setup(a => a.Anonymize(It.IsAny<IDEEvent>())).Throws<NullReferenceException>();

            WhenEverythingIsExported();

            var actual = _publishedEvents.Single();

            Assert.IsInstanceOf<ErrorEvent>(actual);
            Assert.AreEqual("An unrecoverable error occured during anonymization of KaVE.Commons.TestUtils.Model.Events.TestIDEEvent.", ((ErrorEvent)actual).Content);
        }

        [Test]
        public void RaisesExportStarted()
        {
            var exportStarted = false;
            _sut.ExportStarted += () => exportStarted = true;

            WhenEverythingIsExported();

            Assert.IsTrue(exportStarted);
        }

        [Test]
        public void RaisesExportEnded()
        {
            var exportEnded = false;
            _sut.ExportEnded += () => exportEnded = true;

            WhenEverythingIsExported();

            Assert.IsTrue(exportEnded);
        }

        [Test]
        public void ReportsProgress()
        {
            var logEntries = TestEventFactory.SomeEvents(4);
            _logManager.Add(SomeDate, logEntries);
            var actual = new List<int>();
            _sut.ExportProgressChanged += actual.Add;

            WhenEverythingIsExported();

            var expected = new List<int> {25, 50, 75, 100};
            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Entry limit for standard ZIP file should not limit export")]
        public void ExportsMoreThan65535Events()
        {
            var manyEvents = TestEventFactory.SomeEvents(65536);
            _logManager.Add(SomeDate, manyEvents);

            WhenEverythingIsExported();
        }

        [Test, Ignore("Manual Integration Test")]
        public void Manual_Step1_ExportToServer()
        {
            Registry.RegisterComponent<IIoUtils>(new IoUtils());
            _sut.Export(
                DateTime.Now,
                new HttpPublisher(new Uri("http://kave.st.informatik.tu-darmstadt.de/test/upload")));
            Registry.Clear();
        }

        [Test, Ignore("Manual Integration Test")]
        public void Manual_Step2_CheckUploadedFile()
        {
            var actual = ReadEvents(new ZipFile(@"C:\compare.zip"));
            CollectionAssert.AreEquivalent(_eventsForRealLifeExample, actual);
        }

        private int WhenEverythingIsExported()
        {
            return _sut.Export(DateTime.Now, _publisher);
        }

        private void AssertPublishedEvents(params IDEEvent[] expectedEvents)
        {
            AssertPublishedEvents(expectedEvents.ToList());
        }

        private void AssertPublishedEvents(IEnumerable<IDEEvent> expectedEvents)
        {
            CollectionAssert.AreEquivalent(expectedEvents, _publishedEvents);
        }

        private static IEnumerable<IDEEvent> ReadEvents(ZipFile zipFile)
        {
            foreach (var entry in zipFile.Entries)
            {
                using (var jsonStream = new MemoryStream())
                {
                    entry.Extract(jsonStream);
                    var json = jsonStream.AsString();
                    yield return json.ParseJsonTo<IDEEvent>();
                }
            }
        }
    }
}
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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Database;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Database;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests
{
    [TestFixture]
    internal class FeedbackMapperTest : FeedbackDatabaseBasedTest
    {
        private FeedbackMapper _uut;
        private IFeedbackDatabase _targetFeedbackDatabase;

        [SetUp]
        public void SetUp()
        {
            _targetFeedbackDatabase = new TestFeedbackDatabase();
            _uut = new FeedbackMapper(TestFeedbackDatabase, _targetFeedbackDatabase, new NullLogger());
        }

        [TearDown]
        public void TearDown()
        {
            TestProcessor.Instances.Clear();
        }

        [Test]
        public void CleansTargetDatabase()
        {
            _targetFeedbackDatabase.GetDeveloperCollection().Insert(new Developer());
            _targetFeedbackDatabase.GetEventsCollection().Insert(new TestIDEEvent());

            _uut.MapFeedback();

            CollectionAssert.IsEmpty(_targetFeedbackDatabase.GetDeveloperCollection().FindAll());
            CollectionAssert.IsEmpty(_targetFeedbackDatabase.GetEventsCollection().FindAll());
        }

        [Test]
        public void CopiesOverDevelopers()
        {
            var developer1 = GivenDeveloperExists("sessionA", "sessionB");
            var developer2 = GivenDeveloperExists("sessionC");

            _uut.MapFeedback();

            var developers = _targetFeedbackDatabase.GetDeveloperCollection().FindAll();
            CollectionAssert.AreEquivalent(new[] {developer1, developer2}, developers);
        }

        [Test]
        public void CopyingDevelopersPreservesId()
        {
            var developer = GivenDeveloperExists("sessionA");
            var originalId = developer.Id;

            _uut.MapFeedback();

            var developers = _targetFeedbackDatabase.GetDeveloperCollection().FindAll();
            Assert.That(developers.Any(dev => dev.Id.Equals(originalId)));
        }

        [Test]
        public void PassesDevelopersToProcessors()
        {
            var developer = GivenDeveloperExists("session");

            _uut.RegisterMapper(new InactiveProcessor());
            _uut.MapFeedback();

            var processor = TestProcessor.Instances[0];
            Assert.AreEqual(new[] {developer}, processor.ProcessedDevelopers);
        }

        [Test]
        public void PassesEventsToProcessors()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");
            var event2 = GivenEventExists(ideSessionUUID, "2");

            _uut.RegisterMapper(new InactiveProcessor());
            _uut.RegisterMapper(new InactiveProcessor());
            _uut.MapFeedback();

            var testProcessor1 = TestProcessor.Instances[0];
            CollectionAssert.AreEqual(new[] {event1, event2}, testProcessor1.ProcessedEvents);
            var testProcessor2 = TestProcessor.Instances[1];
            CollectionAssert.AreEqual(new[] {event1, event2}, testProcessor2.ProcessedEvents);
        }

        [Test]
        public void PassesOnlyEventsForTheSameDeveloperToOneProcessorInstance()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");
            var event2 = GivenEventExists(ideSessionUUID, "2");
            GivenEventExists("sessionB", "1");

            _uut.RegisterMapper(new InactiveProcessor());
            _uut.MapFeedback();

            var testProcessor = TestProcessor.Instances.First();
            CollectionAssert.AreEqual(new[] {event1, event2}, testProcessor.ProcessedEvents);
        }

        [Test]
        public void KeepsEventIfProcessorIgnoresIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new InactiveProcessor());
            _uut.RegisterMapper(new InactiveProcessor());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll();
            CollectionAssert.AreEquivalent(cleanEvents, new[] {event1});
        }

        [Test]
        public void DropsEventIfProcessorConsumesIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new DroppingProcessor());
            _uut.RegisterMapper(new InactiveProcessor());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll();
            CollectionAssert.IsEmpty(cleanEvents);
        }

        [Test]
        public void ReplacesEventIfProcessorReplacesIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new ReplacingConsumer());
            _uut.RegisterMapper(new InactiveProcessor());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, cleanEvents.Count());
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void ShouldNotDropReplacedEvents()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new ReplacingConsumer());
            _uut.RegisterMapper(new DroppingProcessor());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void ShouldInsertBothEventsWhenTwoProcessorReplaceEvent()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new ReplacingConsumer());
            _uut.RegisterMapper(new ReplacingConsumer());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(2, cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void InsertsNewEventWhenOneProcessorIgnoresIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new InsertingConsumer());
            _uut.RegisterMapper(new InactiveProcessor());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(2, cleanEvents.Count);
        }

        [Test]
        public void InsertsNewEventAndDropsOriginalEventWhenAnotherProcessorDropsIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new InsertingConsumer());
            _uut.RegisterMapper(new DroppingProcessor());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void DropsOriginalEventWhenOneProcessorReplacesAndAnotherInserts()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new InsertingConsumer());
            _uut.RegisterMapper(new ReplacingConsumer());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(2, cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void ShouldNotDuplicateOriginalEvent()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterMapper(new InsertingConsumer());
            _uut.RegisterMapper(new InsertingConsumer());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(3, cleanEvents.Count);
        }

        [Test]
        public void InsertEventsOnStreamEnd()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);

            _uut.RegisterMapper(new OnStreamEndsInsertingConsumer());
            _uut.MapFeedback();

            var cleanEvents = _targetFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(1,cleanEvents.Count);
        }

        private abstract class TestProcessor : BaseEventMapper
        {
            public static readonly IList<TestProcessor> Instances = new List<TestProcessor>();
            public readonly ICollection<IDEEvent> ProcessedEvents = new List<IDEEvent>();
            public readonly ICollection<Developer> ProcessedDevelopers = new List<Developer>();

            protected TestProcessor()
            {
                Instances.Add(this);
                RegisterFor<IDEEvent>(ProcessAnyEvent);
            }

            public override void OnStreamStarts(Developer value)
            {
                ProcessedDevelopers.Add(value);
            }

            public virtual void ProcessAnyEvent(IDEEvent @event)
            {
                ProcessedEvents.Add(@event);
            }
        }

        private class InactiveProcessor : TestProcessor {}

        private class DroppingProcessor : TestProcessor
        {
            public override void ProcessAnyEvent(IDEEvent @event)
            {
                base.ProcessAnyEvent(@event);
                DropCurrentEvent();
            }
        }

        private class ReplacingConsumer : TestProcessor
        {
            public override void ProcessAnyEvent(IDEEvent @event)
            {
                base.ProcessAnyEvent(@event);

                var newEvent = TestEventFactory.SomeEvent();
                newEvent.IDESessionUUID = @event.IDESessionUUID;

                ReplaceCurrentEventWith(newEvent);
            }
        }

        private class InsertingConsumer : TestProcessor
        {
            public override void ProcessAnyEvent(IDEEvent @event)
            {
                base.ProcessAnyEvent(@event);

                var newEvent = TestEventFactory.SomeEvent();
                newEvent.IDESessionUUID = @event.IDESessionUUID;

                Insert(newEvent);
            }
        }

        private class OnStreamEndsInsertingConsumer : TestProcessor
        {
            public override IKaVESet<IDEEvent> OnStreamEnds()
            {
                return Sets.NewHashSet<IDEEvent>(TestEventFactory.SomeEvent());
            }
        }
    }
}
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
 *    - Sven Amann
 *    - Mattis Manfred Kämmerer
 *    - Markus Zimmermann
 */

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup
{
    [TestFixture]
    internal class FeedbackCleanerTest : FeedbackDatabaseBasedTest
    {
        private FeedbackCleaner _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new FeedbackCleaner(TestFeedbackDatabase);
        }

        [TearDown]
        public void TearDown()
        {
            TestProcessor.Instances.Clear();
        }

        [Test]
        public void InstatiatesProcessorsForEachDeveloper()
        {
            GivenDeveloperExists("sessionA");
            GivenDeveloperExists("sessionB");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(2, TestProcessor.Instances.Count);
        }

        [Test]
        public void InstantiatesEachRegisteredProcessorOnce()
        {
            GivenDeveloperExists("sessionA");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(3, TestProcessor.Instances.Count);
        }

        [Test]
        public void PassesEventsToProcessors()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");
            var event2 = GivenEventExists(ideSessionUUID, "2");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

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

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var testProcessor = TestProcessor.Instances.First();
            CollectionAssert.AreEqual(new[] {event1, event2}, testProcessor.ProcessedEvents);
        }

        [Test]
        public void KeepsEventIfProcessorIgnoresIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll();
            CollectionAssert.AreEquivalent(cleanEvents, new[] {event1});
        }

        [Test]
        public void DropsEventIfProcessorConsumesIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ConsumingProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll();
            CollectionAssert.IsEmpty(cleanEvents);
        }

        [Test]
        public void ReplacesEventIfProcessorReplacesIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, cleanEvents.Count());
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void ShouldNotDropReplacedEvents()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.RegisterProcessor<ConsumingProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void ShouldInsertBothEventsWhenTwoProcessorReplaceEvent()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(2, cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void InsertsNewEventWhenOneProcessorIgnoresIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<InsertingConsumer>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(2,cleanEvents.Count);
        }

        [Test]
        public void InsertsNewEventAndDropsOriginalEventWhenAnotherProcessorDropsIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<InsertingConsumer>();
            _uut.RegisterProcessor<ConsumingProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(1,cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents,event1);
        }

        [Test]
        public void DropsOriginalEventWhenOneProcessorReplacesAndAnotherInserts()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<InsertingConsumer>();
            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(2, cleanEvents.Count);
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test]
        public void ShouldNotDuplicateOriginalEvent()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<InsertingConsumer>();
            _uut.RegisterProcessor<InsertingConsumer>();
            _uut.ProcessFeedback();

            var cleanEvents = TestFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(3, cleanEvents.Count);
        }

        private abstract class TestProcessor : IIDEEventProcessor
        {
            public static readonly IList<TestProcessor> Instances = new List<TestProcessor>();
            public readonly ICollection<IDEEvent> ProcessedEvents = new List<IDEEvent>();

            protected TestProcessor()
            {
                Instances.Add(this);
            }

            public virtual ISet<IDEEvent> Process(IDEEvent @event)
            {
                ProcessedEvents.Add(@event);
                return null;
            }
        }

        private class InactiveProcessor : TestProcessor
        {
            public override ISet<IDEEvent> Process(IDEEvent @event)
            {
                base.Process(@event);
                return new KaVEHashSet<IDEEvent> {@event};
            }
        }

        private class ConsumingProcessor : TestProcessor
        {
            public override ISet<IDEEvent> Process(IDEEvent @event)
            {
                base.Process(@event);
                return new KaVEHashSet<IDEEvent>();
            }
        }

        private class ReplacingConsumer : TestProcessor
        {
            public override ISet<IDEEvent> Process(IDEEvent @event)
            {
                base.Process(@event);

                var newEvent = IDEEventTestFactory.SomeEvent();
                newEvent.IDESessionUUID = @event.IDESessionUUID;

                return new KaVEHashSet<IDEEvent> {newEvent};
            }
        }

        private class InsertingConsumer : TestProcessor
        {
            public override ISet<IDEEvent> Process(IDEEvent @event)
            {
                base.Process(@event);

                var newEvent = IDEEventTestFactory.SomeEvent();
                newEvent.IDESessionUUID = @event.IDESessionUUID;

                return new KaVEHashSet<IDEEvent> {@event, newEvent};
            }
        }
    }
}
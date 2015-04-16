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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Cleanup;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Database;
using MongoDB.Bson;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup
{
    [TestFixture]
    internal class FeedbackCleanerTest
    {
        private TestFeedbackDatabase _testFeedbackDatabase;
        private FeedbackCleaner _uut;

        [SetUp]
        public void SetUp()
        {
            _testFeedbackDatabase = new TestFeedbackDatabase();
            _uut = new FeedbackCleaner(_testFeedbackDatabase);
        }

        [TearDown]
        public void TearDown()
        {
            TestProcessor.Instances.Clear();
        }

        [Test]
        public void InstatiatesProcessorsForEachDeveloper()
        {
            GivenDeveloperExists("000000000000000000000001", "sessionA");
            GivenDeveloperExists("000000000000000000000002", "sessionB");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(2, TestProcessor.Instances.Count);
        }

        [Test]
        public void InstantiatesEachRegisteredProcessorOnce()
        {
            GivenDeveloperExists("000000000000000000000001", "sessionA");

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
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");
            var event2 = GivenEventExists(ideSessionUUID, "2");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var testProcessor1 = TestProcessor.Instances[0];
            CollectionAssert.AreEqual(new[] { event1, event2 }, testProcessor1.ProcessedEvents);
            var testProcessor2 = TestProcessor.Instances[1];
            CollectionAssert.AreEqual(new[] { event1, event2 }, testProcessor2.ProcessedEvents);
        }

        [Test]
        public void PassesOnlyEventsForTheSameDeveloperToOneProcessorInstance()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");
            var event2 = GivenEventExists(ideSessionUUID, "2");
            GivenEventExists("sessionB", "1");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var testProcessor = TestProcessor.Instances.First();
            CollectionAssert.AreEqual(new[] { event1, event2 }, testProcessor.ProcessedEvents);
        }

        [Test]
        public void KeepsEventIfProcessorIgnoresIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = _testFeedbackDatabase.GetCleanEventsCollection().FindAll();
            CollectionAssert.AreEquivalent(cleanEvents, new[] {event1});
        }

        [Test]
        public void DropsEventIfProcessorConsumesIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ConsumingProcessor>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = _testFeedbackDatabase.GetCleanEventsCollection().FindAll();
            CollectionAssert.IsEmpty(cleanEvents);
        }

        [Test]
        public void ReplacesEventIfProcessorReplacesIt()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.RegisterProcessor<InactiveProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = _testFeedbackDatabase.GetCleanEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, cleanEvents.Count());
            CollectionAssert.DoesNotContain(cleanEvents, event1);
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "cannot drop and replace an event")]
        public void ThrowsIfOneProcessorReplacesAndOneConsumesAnEvent()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.RegisterProcessor<ConsumingProcessor>();
            _uut.ProcessFeedback();
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "cannot replace an event by two")]
        public void ThrowsIfTwoProcessorsReplaceAnEvent()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            GivenEventExists(ideSessionUUID, "1");

            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.RegisterProcessor<ReplacingConsumer>();
            _uut.ProcessFeedback();
        }

        private TestIDEEvent GivenEventExists(String sessionId, String value)
        {
            var testIDEEvent = new TestIDEEvent { IDESessionUUID = sessionId, TestProperty = value };
            var ideEventCollection = _testFeedbackDatabase.GetOriginalEventsCollection();
            ideEventCollection.Insert(testIDEEvent);
            return testIDEEvent;
        }

        /// <param name="developerId">must be 24 characters, hex</param>
        /// <param name="sessionIds">should be disjunct from any other developer's ids</param>
        private void GivenDeveloperExists(String developerId, params String[] sessionIds)
        {
            var developer = new Developer {Id = new ObjectId(developerId)};
            foreach (var sessionId in sessionIds)
            {
                developer.SessionIds.Add(sessionId);
            }
            _testFeedbackDatabase.GetDeveloperCollection().Insert(developer);
        }

        private abstract class TestProcessor : IIDEEventProcessor
        {
            public static readonly IList<TestProcessor> Instances = new List<TestProcessor>();
            public readonly ICollection<IDEEvent> ProcessedEvents = new List<IDEEvent>();

            public TestProcessor()
            {
                Instances.Add(this);
            }

            public virtual IDEEvent Process(IDEEvent @event)
            {
                ProcessedEvents.Add(@event);
                return null;
            }
        }

        private class InactiveProcessor : TestProcessor
        {
            public override IDEEvent Process(IDEEvent @event)
            {
                base.Process(@event);
                return @event;
            }
        }

        private class ConsumingProcessor : TestProcessor
        {
            public override IDEEvent Process(IDEEvent @event)
            {
                base.Process(@event);
                return null;
            }
        }

        private class ReplacingConsumer : TestProcessor
        {
            public override IDEEvent Process(IDEEvent @event)
            {
                base.Process(@event);
                var testIDEEvent = (TestIDEEvent) @event;
                return new TestIDEEvent
                {
                    IDESessionUUID = @event.IDESessionUUID,
                    TestProperty = testIDEEvent.TestProperty + "_modified"
                };
            }
        }
    }
}
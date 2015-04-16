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

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(2, TestProcessor.Instances.Count);
        }

        [Test]
        public void InstantiatesEachRegisteredProcessorOnce()
        {
            GivenDeveloperExists("000000000000000000000001", "sessionA");

            _uut.RegisterProcessor<TestProcessor>();
            _uut.RegisterProcessor<TestProcessor>();
            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(3, TestProcessor.Instances.Count);
        }

        [Test]
        public void PassesEventsToProcessors()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var events = new[]
            {
                new TestIDEEvent {IDESessionUUID = ideSessionUUID, Id = "1"},
                new TestIDEEvent {IDESessionUUID = ideSessionUUID, Id = "2"}
            };
            GivenEventExists(events[0]);
            GivenEventExists(events[1]);

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            var testProcessor = TestProcessor.Instances.First();
            CollectionAssert.AreEqual(events, testProcessor.ProcessedEvents);
        }

        [Test]
        public void PassesOnlyEventsForTheSameDeveloperToOneProcessorInstance()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var events = new[]
            {
                new TestIDEEvent {IDESessionUUID = ideSessionUUID, Id = "1"},
                new TestIDEEvent {IDESessionUUID = ideSessionUUID, Id = "2"}
            };
            GivenEventExists(events[0]);
            GivenEventExists(events[1]);
            GivenEventExists(new TestIDEEvent { IDESessionUUID = "sessionB", Id = "1" });

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            var testProcessor = TestProcessor.Instances.First();
            CollectionAssert.AreEqual(events, testProcessor.ProcessedEvents);
        }

        [Test]
        public void WritesReturnedEventToCleanCollection()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var @event = new TestIDEEvent { IDESessionUUID = ideSessionUUID, Id = "1" };
            GivenEventExists(@event);

            _uut.RegisterProcessor<SameEventReturntingProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = _testFeedbackDatabase.GetCleanEventsCollection().FindAll();
            CollectionAssert.AreEquivalent(cleanEvents, new[]{@event});
        }

        [Test]
        public void DropsEventIfProcessorReturnsNull()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists("000000000000000000000001", ideSessionUUID);
            var @event = new TestIDEEvent { IDESessionUUID = ideSessionUUID, Id = "1" };
            GivenEventExists(@event);

            _uut.RegisterProcessor<NullReturningProcessor>();
            _uut.ProcessFeedback();

            var cleanEvents = _testFeedbackDatabase.GetCleanEventsCollection().FindAll();
            CollectionAssert.IsEmpty(cleanEvents);
        }

        private void GivenEventExists(TestIDEEvent infoEvent)
        {
            var ideEventCollection = _testFeedbackDatabase.GetOriginalEventsCollection();
            ideEventCollection.Insert(infoEvent);
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

        private class TestProcessor : IIDEEventProcessor
        {
            public static readonly ICollection<TestProcessor> Instances = new List<TestProcessor>();
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

        private class SameEventReturntingProcessor : TestProcessor
        {
            public override IDEEvent Process(IDEEvent @event)
            {
                base.Process(@event);
                return @event;
            }
        }

        private class NullReturningProcessor : TestProcessor
        {
            public override IDEEvent Process(IDEEvent @event)
            {
                base.Process(@event);
                return null;
            }
        }
    }
}
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
using System.Runtime.Serialization;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Database;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests
{
    [TestFixture]
    internal class FeedbackCleanerTest
    {
        private TestFeedbackDatabase _testFeedbackDatabase;
        private FeedbackCleaner _uut;

        [SetUp]
        public void SetUpDatabase()
        {
            _testFeedbackDatabase = new TestFeedbackDatabase();
            _uut = new FeedbackCleaner(_testFeedbackDatabase);
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
            GivenEventExists(new InfoEvent{IDESessionUUID = ideSessionUUID, Info = "1"});
            GivenEventExists(new InfoEvent{IDESessionUUID = ideSessionUUID, Info = "2"});

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(1, TestProcessor.Instances.Count);
            var testProcessor = TestProcessor.Instances.First();
            Assert.AreEqual(2, testProcessor.ProcessedEvents.Count);
        }

        private void GivenEventExists(InfoEvent infoEvent)
        {
            var ideEventCollection = _testFeedbackDatabase.GetEventsCollection();
            ideEventCollection.Insert(infoEvent);
        }

        /// <param name="developerId">must be 24 characters, hex</param>
        /// <param name="sessionIds"></param>
        private void GivenDeveloperExists(String developerId, params String[] sessionIds)
        {
            var developer = new Developer { Id = new ObjectId(developerId) };
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

            public IDEEvent Process(IDEEvent @event)
            {
                ProcessedEvents.Add(@event);
                return null;
            }
        }

        private class TestFeedbackDatabase : IFeedbackDatabase
        {
            private readonly IDeveloperCollection _developerCollection = new TestDeveloperCollection();
            private readonly IIDEEventCollection _ideEventCollection = new TestIDEEventCollection();

            public IDeveloperCollection GetDeveloperCollection()
            {
                return _developerCollection;
            }

            public IIDEEventCollection GetEventsCollection()
            {
                return _ideEventCollection;
            }
        }

        private class TestDeveloperCollection : IDeveloperCollection
        {
            private readonly ICollection<Developer> _developers = new List<Developer>();

            public IEnumerable<Developer> FindAll()
            {
                return _developers;
            }

            public void Insert(Developer instance)
            {
                _developers.Add(instance);
            }

            public void Save(Developer instance)
            {
                throw new System.NotImplementedException();
            }

            public IList<Developer> FindBySessionId(string sessionId)
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestIDEEventCollection : IIDEEventCollection
        {
            private readonly ICollection<IDEEvent> _ideEvents = new List<IDEEvent>(); 

            public IEnumerable<IDEEvent> FindAll()
            {
                throw new System.NotImplementedException();
            }

            public void Insert(IDEEvent instance)
            {
                _ideEvents.Add(instance);
            }

            public void Save(IDEEvent instance)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<IDEEvent> GetEventStream(Developer developer)
            {
                return _ideEvents;
            }

            public bool Contains(IDEEvent @event)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
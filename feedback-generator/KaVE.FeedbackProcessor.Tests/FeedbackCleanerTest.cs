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

using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Database;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests
{
    [TestFixture]
    internal class FeedbackCleanerTest
    {
        private TestDatabase _testDatabase;
        private FeedbackCleaner _uut;

        [SetUp]
        public void SetUpDatabase()
        {
            _testDatabase = new TestDatabase();
            _uut = new FeedbackCleaner(_testDatabase);
        }

        [Test]
        public void InstatiatesProcessorsForEachDeveloper()
        {
            var developerCollection = _testDatabase.GetDeveloperCollection();
            developerCollection.Insert(new Developer());
            developerCollection.Insert(new Developer());

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(2, TestProcessor.Instances.Count);
        }

        [Test]
        public void PassesEventsToProcessors()
        {
            
        }

        private class TestProcessor : IIDEEventProcessor
        {
            public static readonly ICollection<TestProcessor> Instances = new List<TestProcessor>();

            public TestProcessor()
            {
                Instances.Add(this);
            }

            public IDEEvent Process(IDEEvent @event)
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestDatabase : IFeedbackDatabase
        {
            private readonly IDeveloperCollection _developerCollection;

            public TestDatabase()
            {
                _developerCollection = new TestDeveloperCollection();
            }

            public IDeveloperCollection GetDeveloperCollection()
            {
                return _developerCollection;
            }

            public IIDEEventCollection GetEventsCollection()
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestDeveloperCollection : IDeveloperCollection
        {
            private readonly ICollection<Developer> _developers;
 
            public TestDeveloperCollection()
            {
                _developers = new List<Developer>();
            }

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
    }
}
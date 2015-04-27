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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Statistics;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class FeedbackWalkerTest : FeedbackDatabaseBasedTest
    {
        private FeedbackWalker _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new FeedbackWalker(TestFeedbackDatabase);
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

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(2, TestProcessor.Instances.Count);
        }

        [Test]
        public void InstantiatesEachRegisteredProcessorOnce()
        {
            GivenDeveloperExists("sessionA");

            _uut.RegisterProcessor<TestProcessor>();
            _uut.RegisterProcessor<TestProcessor>();
            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            Assert.AreEqual(3, TestProcessor.Instances.Count);
        }

        [Test]
        public void SetsDeveloperOnProcessor()
        {
            var developer1 = GivenDeveloperExists("session1");
            var developer2 = GivenDeveloperExists("session2");

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            var processor1 = TestProcessor.Instances[0];
            Assert.AreEqual(developer1, processor1.Developer);
            var processor2 = TestProcessor.Instances[1];
            Assert.AreEqual(developer2, processor2.Developer);
        }

        [Test]
        public void PassesEventsToProcessors()
        {
            const string ideSessionUUID = "sessionA";
            GivenDeveloperExists(ideSessionUUID);
            var event1 = GivenEventExists(ideSessionUUID, "1");
            var event2 = GivenEventExists(ideSessionUUID, "2");

            _uut.RegisterProcessor<TestProcessor>();
            _uut.RegisterProcessor<TestProcessor>();
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

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            var testProcessor = TestProcessor.Instances.First();
            CollectionAssert.AreEqual(new[] {event1, event2}, testProcessor.ProcessedEvents);
        }

        [Test]
        public void InformsProcessorsWhenStreamEnds()
        {
            GivenDeveloperExists("sessionX");
            GivenEventExists("sessionX", "A");
            GivenEventExists("sessionX", "B");

            _uut.RegisterProcessor<TestProcessor>();
            _uut.ProcessFeedback();

            var processor = TestProcessor.Instances[0];
            Assert.IsTrue(processor.IsFinilized);
        }

        private class TestProcessor : IIDEEventProcessor
        {
            public  static readonly IList<TestProcessor> Instances = new List<TestProcessor>();
            public readonly IList<IDEEvent> ProcessedEvents = new List<IDEEvent>();
            public bool IsFinilized;

            public TestProcessor()
            {
                Instances.Add(this);
            }

            public Developer Developer { set; get; }

            public void Process(IDEEvent @event)
            {
                ProcessedEvents.Add(@event);
            }

            public void OnStreamEnds()
            {
                IsFinilized = true;
            }
        }
    }
}
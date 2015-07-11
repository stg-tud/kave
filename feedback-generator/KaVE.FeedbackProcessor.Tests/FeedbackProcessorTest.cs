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
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests
{
    [TestFixture]
    internal class FeedbackProcessorTest : FeedbackDatabaseBasedTest
    {
        private FeedbackProcessor _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new FeedbackProcessor(TestFeedbackDatabase, new NullLogger());
        }

        [Test]
        public void SetsDeveloperOnProcessor()
        {
            var developer1 = GivenDeveloperExists("session1");
            var developer2 = GivenDeveloperExists("session2");

            var testProcessor = new TestProcessor();
            _uut.Register(testProcessor);
            _uut.ProcessFeedback();

            CollectionAssert.AreEqual(new[] {developer1, developer2}, testProcessor.ProcessedDevelopers);
        }

        [Test]
        public void PassesEventsToProcessors()
        {
            GivenDeveloperExists("sessionA");
            var event1 = GivenEventExists("sessionA", "1");
            var event2 = GivenEventExists("sessionA", "2");

            var testProcessor1 = new TestProcessor();
            _uut.Register(testProcessor1);
            var testProcessor2 = new TestProcessor();
            _uut.Register(testProcessor2);
            _uut.ProcessFeedback();

            CollectionAssert.AreEqual(new[] {event1, event2}, testProcessor1.ProcessedEvents);
            CollectionAssert.AreEqual(new[] {event1, event2}, testProcessor2.ProcessedEvents);
        }

        [Test]
        public void PassesOnlyEventsOfCurrentDeveloperToProcessor()
        {
            GivenDeveloperExists("sessionA");
            GivenEventExists("sessionA", "1");
            GivenEventExists("sessionA", "2");
            var isolatedEvent = GivenEventExists("sessionB", "1");

            var testProcessor = new TestProcessor();
            _uut.Register(testProcessor);
            _uut.ProcessFeedback();

            // Asserts events belong to current developer in TestProcessor.OnEvent(IDEEvent)
            CollectionAssert.DoesNotContain(testProcessor.ProcessedEvents, isolatedEvent);
        }

        [Test]
        public void InformsProcessorsWhenStreamForCurrentDeveloperEnds()
        {
            GivenDeveloperExists("sessionX");
            GivenEventExists("sessionX", "A");
            GivenEventExists("sessionX", "B");

            var testProcessor = new TestProcessor();
            _uut.Register(testProcessor);
            _uut.ProcessFeedback();

            Assert.IsTrue(testProcessor.IsFinilized);
        }

        private class TestProcessor : IEventProcessor
        {
            public readonly IList<IDEEvent> ProcessedEvents = new List<IDEEvent>();
            public readonly IList<Developer> ProcessedDevelopers = new List<Developer>();
            public bool IsFinilized;

            public void OnStreamStarts(Developer developer)
            {
                ProcessedDevelopers.Add(developer);
            }

            public void OnEvent(IDEEvent @event)
            {
                CollectionAssert.Contains(ProcessedDevelopers.Last().SessionIds, @event.IDESessionUUID);
                ProcessedEvents.Add(@event);
            }

            public void OnStreamEnds()
            {
                IsFinilized = true;
            }
        }
    }
}
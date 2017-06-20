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
using System.Linq;
using KaVE.Commons.TestUtils.Model.Events;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Import
{
    [TestFixture]
    internal class FeedbackImporterFilterDuplicatesTest : FeedbackImporterTestBase
    {
        [Test]
        public void FiltersDuplicate()
        {
            var @event = new TestIDEEvent
            {
                IDESessionUUID = "sessionA",
                TestProperty = "a",
                TriggeredAt = new DateTime()
            };
            GivenInputArchive("0.zip").With("0.json", @event).With("1.json", @event);

            WhenImportIsRun();

            Assert.AreEqual(1, TestFeedbackDatabase.GetEventsCollection().FindAll().Count());
        }

        [Test]
        public void ShouldUseSessionIdOfFilteredEventsToFindDeveloper()
        {
            var @event = new TestIDEEvent
            {
                IDESessionUUID = "sessionA",
                TestProperty = "a",
                TriggeredAt = new DateTime()
            };
            GivenInputArchive("0.zip").With("0.json", @event);
            GivenInputArchive("1.zip")
                .With("0.json", @event)
                .With("1.json", new TestIDEEvent {IDESessionUUID = "sessionB", TestProperty = "b"});

            WhenImportIsRun();

            var developers = TestFeedbackDatabase.GetDeveloperCollection().FindAll().ToList();
            Assert.AreEqual(1, developers.Count);
            var developer = developers[0];
            CollectionAssert.AreEquivalent(new[] {"sessionA", "sessionB"}, developer.SessionIds);
        }
    }
}
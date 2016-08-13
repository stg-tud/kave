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
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Import
{
    internal class FeedbackImporterWithoutTriggerTimesTest : FeedbackImporterTestBase
    {
        [Test]
        public void SetsTriggerTimeIfNotSet()
        {
            GivenInputArchive("1.zip").With("0.json", new TestIDEEvent {TriggeredAt = null});

            WhenImportIsRun();

            var ideEvent = GetSingleEventFromDatabase();
            Assert.NotNull(ideEvent.TriggeredAt);
        }

        [Test]
        public void DoesNotOverwriteExistingTriggerTime()
        {
            var originalTriggerTime = new DateTime(2015, 4, 21);
            GivenInputArchive("1.zip").With("0.json", new TestIDEEvent {TriggeredAt = originalTriggerTime});

            WhenImportIsRun();

            var ideEvent = GetSingleEventFromDatabase();
            Assert.AreEqual(originalTriggerTime, ideEvent.TriggeredAt);
        }

        [Test]
        public void PreservessOrderFromArchiveWithArtificialTriggerTimes()
        {
            GivenInputArchive("1.zip")
                .With("0.json", new TestIDEEvent {TriggeredAt = null, TestProperty = "23"})
                .With("1.json", new TestIDEEvent {TriggeredAt = null, TestProperty = "1"})
                .With("2.json", new TestIDEEvent {TriggeredAt = null, TestProperty = "42"});

            WhenImportIsRun();

            var ideEvents =
                TestFeedbackDatabase.GetEventsCollection()
                                    .FindAll()
                                    .Cast<TestIDEEvent>()
                                    .OrderBy(evt => evt.TriggeredAt)
                                    .ToList();
            Assert.AreEqual(3, ideEvents.Count);
            Assert.Less(ideEvents[0].TriggeredAt, ideEvents[1].TriggeredAt);
            Assert.Less(ideEvents[1].TriggeredAt, ideEvents[2].TriggeredAt);
        }

        private IDEEvent GetSingleEventFromDatabase()
        {
            var ideEvents = TestFeedbackDatabase.GetEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, ideEvents.Count);
            return ideEvents[0];
        }
    }
}
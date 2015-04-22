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
using System.Linq;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Import
{
    [TestFixture]
    internal class FeedbackImporterIDEStateEventNoSessionIdBugTest : FeedbackImporterTestBase
    {
        private readonly TestIDEEvent _completeEvent = new TestIDEEvent
        {
            IDESessionUUID = "a"
        };

        private readonly IDEStateEvent _brokenShutdownEvent = new IDEStateEvent
        {
            IDESessionUUID = null,
            TriggeredAt = DateTime.Now /* just any time */,
            IDELifecyclePhase = IDEStateEvent.LifecyclePhase.Shutdown
        };

        [Test]
        public void SetsSessionIdOfIDEStateEventToSessionIdOfPreviousEventIfNotSet()
        {
            GivenInputArchive("0.zip")
                .With("0.json", _completeEvent)
                .With("1.json", _brokenShutdownEvent);

            WhenImportIsRun();

            var expected = new IDEStateEvent
            {
                IDESessionUUID = _completeEvent.IDESessionUUID,
                TriggeredAt = _brokenShutdownEvent.TriggeredAt,
                IDELifecyclePhase = _brokenShutdownEvent.IDELifecyclePhase
            };
            CollectionAssert.Contains(TestFeedbackDatabase.GetOriginalEventsCollection().FindAll(), expected);
        }

        [Test,
         Ignore("We don't have an archive with a broken state event on 1st position, so we don't need a fix for this")]
        public void SetsSessionIdOfIDEStateEventToSessionIdOfSubsequentEventIfNotSetAndNoPreviousEventExists()
        {
            GivenInputArchive("0.zip")
                .With("0.json", _brokenShutdownEvent)
                .With("1.json", _completeEvent);

            WhenImportIsRun();

            var expected = new IDEStateEvent
            {
                IDESessionUUID = _completeEvent.IDESessionUUID,
                TriggeredAt = _brokenShutdownEvent.TriggeredAt,
                IDELifecyclePhase = _brokenShutdownEvent.IDELifecyclePhase
            };
            CollectionAssert.Contains(TestFeedbackDatabase.GetOriginalEventsCollection().FindAll(), expected);
        }

        [Test]
        public void TreatsIDEStateEventAsAnonymousEventIfWholeArchiveIsAnonymous()
        {
            var anonymousEvent = new TestIDEEvent {IDESessionUUID = null};
            GivenInputArchive("0.zip")
                .With("0.json", anonymousEvent)
                .With("1.json", _brokenShutdownEvent);

            WhenImportIsRun();

            var onlyDeveloper = TestFeedbackDatabase.GetDeveloperCollection().FindAll().First();
            var expected = new IDEStateEvent
            {
                IDESessionUUID = onlyDeveloper.Id.ToString(),
                TriggeredAt = _brokenShutdownEvent.TriggeredAt,
                IDELifecyclePhase = _brokenShutdownEvent.IDELifecyclePhase
            };

            var ideEvents = TestFeedbackDatabase.GetOriginalEventsCollection().FindAll();
            CollectionAssert.Contains(ideEvents, expected);
        }
    }
}
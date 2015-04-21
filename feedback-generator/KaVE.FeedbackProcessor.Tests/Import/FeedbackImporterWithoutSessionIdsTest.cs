﻿/*
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

using System.Linq;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Import
{
    [TestFixture]
    internal class FeedbackImporterWithoutSessionIdsTest : FeedbackImporterTestBase
    {
        [Test]
        public void CreatesDeveloperForEventsFromArchiveWithoutSessionIds()
        {
            GivenInputArchive("0.zip").With("0.json", new TestIDEEvent {IDESessionUUID = null});

            WhenImportIsRun();

            var developer = GetSingleDeveloperFromDatabase();
            CollectionAssert.IsEmpty(developer.SessionIds);
        }

        [Test]
        public void UsesAnonymousDeveloperIdAsSessionIdForEventsWithoutSessionId()
        {
            GivenInputArchive("0.zip").With("0.json", new TestIDEEvent {IDESessionUUID = null});

            WhenImportIsRun();

            var ideEvents = TestFeedbackDatabase.GetOriginalEventsCollection().FindAll().ToList();
            Assert.AreEqual(1, ideEvents.Count);
            var ideEvent = ideEvents[0];
            var developer = GetSingleDeveloperFromDatabase();
            Assert.AreEqual(developer.Id.ToString(), ideEvent.IDESessionUUID);
        }

        private Developer GetSingleDeveloperFromDatabase()
        {
            var developers = TestFeedbackDatabase.GetDeveloperCollection().FindAll().ToList();
            Assert.AreEqual(1, developers.Count);
            return developers[0];
        }
    }
}
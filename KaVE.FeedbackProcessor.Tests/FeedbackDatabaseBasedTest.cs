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
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Database;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests
{
    [TestFixture]
    internal abstract class FeedbackDatabaseBasedTest {

        protected TestFeedbackDatabase TestFeedbackDatabase { get; private set; }

        [SetUp]
        public void SetUpFeedbackDatabases()
        {
            TestFeedbackDatabase = new TestFeedbackDatabase();
        }

        protected TestIDEEvent GivenEventExists(String sessionId, String value = "", DateTime? triggeredAt = null)
        {
            var testIDEEvent = new TestIDEEvent { IDESessionUUID = sessionId, TestProperty = value, TriggeredAt = triggeredAt};
            var ideEventCollection = TestFeedbackDatabase.GetEventsCollection();
            ideEventCollection.Insert(testIDEEvent);
            return testIDEEvent;
        }

        /// <param name="sessionIds">should be disjunct from any other developer's ids</param>
        protected Developer GivenDeveloperExists(params string[] sessionIds)
        {
            var developer = new Developer();
            foreach (var sessionId in sessionIds)
            {
                developer.SessionIds.Add(sessionId);
            }
            TestFeedbackDatabase.GetDeveloperCollection().Insert(developer);
            return developer;
        }
    }
}
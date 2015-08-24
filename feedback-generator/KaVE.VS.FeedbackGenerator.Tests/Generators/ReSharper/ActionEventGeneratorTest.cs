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

using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Generators.ReSharper;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.ReSharper
{
    internal class ActionEventGeneratorTest : EventGeneratorTestBase
    {
        private ActionEventGenerator _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new ActionEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);
        }

        [Test]
        public void ShouldFireEventOnAction()
        {
            const string actionId = "SomeActionId";

            _uut.TrackAction(actionId);

            var publishedEvent = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(actionId, publishedEvent.CommandId);
        }

        [Test]
        public void ShouldFireEventOnActivity()
        {
            const string activityGroup = "SomeActivityGroup";
            const string activityId = "SomeActivityId";

            _uut.TrackActivity(activityGroup, activityId);

            var publishedEvent = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(activityId, publishedEvent.CommandId);
        }
    }
}
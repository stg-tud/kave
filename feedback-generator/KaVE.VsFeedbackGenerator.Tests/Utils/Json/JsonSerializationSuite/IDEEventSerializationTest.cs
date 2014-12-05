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
 *    - Dennis Albrecht
 */

using System;
using KaVE.Model.Events;
using KaVE.Model.Names.VisualStudio;
using KaVE.TestUtils.Model.Events;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
{
    [TestFixture]
    class IDEEventSerializationTest
    {
        [Test]
        public void ShouldSerializeAllFieldsOfEvent()
        {
            var ideEvent = new TestIDEEvent
            {
                IDESessionUUID = "0xDEADBEEF",
                KaVEVersion = "1.2.3.4",
                ActiveDocument = DocumentName.Get("Random"),
                ActiveWindow = WindowName.Get("Random"),
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = DateTime.Now,
                Duration = TimeSpan.FromSeconds(5)
            };

            JsonAssert.SerializationPreservesData(ideEvent);
        }

        [Test]
        public void ShouldSerializeToString()
        {
            var ideEvent = new TestIDEEvent
            {
                ActiveDocument = DocumentName.Get("SomeDocument"),
                ActiveWindow = WindowName.Get("SomeWindow"),
                Duration = new TimeSpan(0, 0, 1),
                IDESessionUUID = "0xDEADBEEF",
                KaVEVersion = "1.2.3.4",
                TriggeredAt = new DateTime(2010, 01, 01, 12, 30, 44),
                TriggeredBy = IDEEvent.Trigger.Click
            };

            const string expected =
                "{\"$type\":\"KaVE.TestUtils.Model.Events.TestIDEEvent, KaVE.TestUtils\",\"IDESessionUUID\":\"0xDEADBEEF\",\"KaVEVersion\":\"1.2.3.4\",\"TriggeredAt\":\"2010-01-01T12:30:44\",\"TriggeredBy\":1,\"Duration\":\"00:00:01\",\"ActiveWindow\":\"VisualStudio.WindowName:SomeWindow\",\"ActiveDocument\":\"VisualStudio.DocumentName:SomeDocument\"}";

            JsonAssert.SerializesTo(ideEvent, expected);
        }

        [Test]
        public void ShouldDeserializeFromWithDurationSet()
        {
            var triggeredAt = new DateTime(2014, 5, 5, 12, 0, 2, 69, DateTimeKind.Local);
            var terminatedAt = new DateTime(2014, 5, 5, 12, 0, 7, 69, DateTimeKind.Local);
            var duration = TimeSpan.FromSeconds(5);
            var eventJson = string.Format("{{\"TriggeredAt\":\"2014-05-05T12:00:02.069000+02:00\",\"Duration\":\"00:00:05\"}}");

            var actual = eventJson.ParseJsonTo<TestIDEEvent>();

            Assert.AreEqual(duration, actual.Duration);
            Assert.AreEqual(triggeredAt, actual.TriggeredAt);
            Assert.AreEqual(terminatedAt, actual.TerminatedAt);
        }

        [Test, Ignore("see todo comment in IDEEvent constructor")]
        public void ShouldNotHaveStartTimeWhenNotSerialized()
        {
            var actual = "{}".ParseJsonTo<TestIDEEvent>();

            Assert.IsNull(actual.TriggeredAt);
        }
    }
}

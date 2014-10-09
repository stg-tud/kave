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
 *    - Dennis Albrecht
 */

using System;
using KaVE.Model.Events;
using KaVE.Model.Names.VisualStudio;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
{
    [TestFixture]
    internal class CommandEventSerializationTest
    {
        [Test]
        public void ShouldSerializeToString()
        {
            var commandEvent = new CommandEvent
            {
                ActiveDocument = DocumentName.Get("SomeDocument"),
                ActiveWindow = WindowName.Get("SomeWindow"),
                CommandId = "SomeId",
                Duration = new TimeSpan(0, 0, 1),
                IDESessionUUID = "0xDEADBEEF",
                TriggeredAt = new DateTime(2010, 01, 01, 12, 30, 44),
                TriggeredBy = IDEEvent.Trigger.Click
            };
            const string expected =
                "{\"$type\":\"KaVE.Model.Events.CommandEvent, KaVE.Model\",\"CommandId\":\"SomeId\",\"IDESessionUUID\":\"0xDEADBEEF\",\"TriggeredAt\":\"2010-01-01T12:30:44\",\"TriggeredBy\":1,\"Duration\":\"00:00:01\",\"ActiveWindow\":\"VisualStudio.WindowName:SomeWindow\",\"ActiveDocument\":\"VisualStudio.DocumentName:SomeDocument\"}";

            JsonAssert.SerializesTo(commandEvent, expected);
        }
    }
}
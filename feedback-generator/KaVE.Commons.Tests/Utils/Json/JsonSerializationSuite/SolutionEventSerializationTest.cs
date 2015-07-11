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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite
{
    [TestFixture]
    internal class SolutionEventSerializationTest
    {
        [Test]
        public void ShoudlSerializeToString()
        {
            var solutionEvent = new SolutionEvent
            {
                ActiveDocument = DocumentName.Get("SomeDocument"),
                ActiveWindow = WindowName.Get("SomeWindow"),
                Action = SolutionEvent.SolutionAction.RenameSolution,
                Duration = new TimeSpan(0, 0, 1),
                IDESessionUUID = "0xDEADBEEF",
                Target = SolutionName.Get("SomeSolution"),
                TriggeredAt = new System.DateTime(2010, 01, 01, 12, 30, 44),
                TriggeredBy = IDEEvent.Trigger.Click
            };
            const string expected =
                "{\"$type\":\"KaVE.Commons.Model.Events.VisualStudio.SolutionEvent, KaVE.Commons\",\"Action\":1,\"Target\":\"VisualStudio.SolutionName:SomeSolution\",\"IDESessionUUID\":\"0xDEADBEEF\",\"TriggeredAt\":\"2010-01-01T12:30:44\",\"TriggeredBy\":1,\"Duration\":\"00:00:01\",\"ActiveWindow\":\"VisualStudio.WindowName:SomeWindow\",\"ActiveDocument\":\"VisualStudio.DocumentName:SomeDocument\"}";

            JsonAssert.SerializesTo(solutionEvent, expected);
        }
    }
}
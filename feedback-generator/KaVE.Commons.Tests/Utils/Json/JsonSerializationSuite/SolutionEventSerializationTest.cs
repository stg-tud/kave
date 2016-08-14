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
using KaVE.Commons.Model.Naming;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite
{
    internal class SolutionEventSerializationTest
    {
        [Test]
        public void ShouldSerializeToString()
        {
            var solutionEvent = new SolutionEvent
            {
                ActiveDocument = Names.Document("d SomeDocument"),
                ActiveWindow = Names.Window("w SomeWindow"),
                Action = SolutionAction.RenameSolution,
                Duration = new TimeSpan(0, 0, 1),
                IDESessionUUID = "0xDEADBEEF",
                Target = Names.Solution("SomeSolution"),
                TriggeredAt = new System.DateTime(2010, 01, 01, 12, 30, 44),
                TriggeredBy = EventTrigger.Click
            };
            const string expected =
                "{\"$type\":\"KaVE.Commons.Model.Events.VisualStudio.SolutionEvent, KaVE.Commons\",\"Action\":1,\"Target\":\"0Sln:SomeSolution\",\"IDESessionUUID\":\"0xDEADBEEF\",\"TriggeredAt\":\"2010-01-01T12:30:44\",\"TriggeredBy\":1,\"Duration\":\"00:00:01\",\"ActiveWindow\":\"0Win:w SomeWindow\",\"ActiveDocument\":\"0Doc:d SomeDocument\"}";

            JsonAssert.SerializesTo(solutionEvent, expected);
        }
    }
}
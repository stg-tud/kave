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
using KaVE.Commons.Model.Events.VisualStudio;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite
{
    internal class BuildEventSerializationTest
    {
        [Test]
        public void ShouldSerializeToString()
        {
            var e = new BuildEvent
            {
                Action = "SomeAction",
                Scope = "SomeScope",
                Targets = new[]
                {
                    new BuildTarget
                    {
                        Duration = new TimeSpan(0, 0, 1),
                        Platform = "SomePlatform",
                        Project = "SomeProject",
                        ProjectConfiguration = "SomeConfiguration",
                        SolutionConfiguration = "SomeOtherConfiguration",
                        StartedAt = new DateTime(2014, 01, 01, 13, 45, 54),
                        Successful = true
                    }
                }
            };
            const string expected =
                "{\"$type\":\"KaVE.Commons.Model.Events.VisualStudio.BuildEvent, KaVE.Commons\",\"Scope\":\"SomeScope\",\"Action\":\"SomeAction\",\"Targets\":[{\"$type\":\"KaVE.Commons.Model.Events.VisualStudio.BuildTarget, KaVE.Commons\",\"Project\":\"SomeProject\",\"ProjectConfiguration\":\"SomeConfiguration\",\"Platform\":\"SomePlatform\",\"SolutionConfiguration\":\"SomeOtherConfiguration\",\"StartedAt\":\"2014-01-01T13:45:54\",\"Duration\":\"00:00:01\",\"Successful\":true}],\"TriggeredBy\":0}";

            JsonAssert.SerializesTo(e, expected);
            JsonAssert.DeserializesTo(expected, e);
        }
    }
}
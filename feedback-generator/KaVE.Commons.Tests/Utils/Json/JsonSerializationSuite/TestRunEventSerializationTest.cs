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
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite
{
    internal class TestRunEventSerializationTest
    {
        [Test]
        public void O2O()
        {
            var a = GetExample();
            var json = a.ToCompactJson();
            var b = json.ParseJsonTo<TestRunEvent>();
            Assert.AreEqual(a, b);
        }

        [Test]
        public void O2J()
        {
            var a = GetExample();
            var json = a.ToCompactJson();
            var expected = GetExampleJson();
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void J2O()
        {
            var json = GetExampleJson();
            var a = json.ParseJsonTo<TestRunEvent>();
            var b = GetExample();
            Assert.AreEqual(a, b);
        }

        private static TestRunEvent GetExample()
        {
            return new TestRunEvent
            {
                WasAborted = true,
                Tests =
                {
                    new TestCaseResult
                    {
                        TestMethod = Names.Method("[T,P] [T,P].M()"),
                        Parameters = "asd",
                        Duration = TimeSpan.FromSeconds(3),
                        Result = TestResult.Ignored
                    }
                }
            };
        }

        private static string GetExampleJson()
        {
            return
                "{\"$type\":\"KaVE.Commons.Model.Events.TestRunEvents.TestRunEvent, KaVE.Commons\",\"WasAborted\":true,\"Tests\":[{\"$type\":\"KaVE.Commons.Model.Events.TestRunEvents.TestCaseResult, KaVE.Commons\",\"TestMethod\":\"0M:[T,P] [T,P].M()\",\"Parameters\":\"asd\",\"Duration\":\"00:00:03\",\"Result\":4}],\"TriggeredBy\":0}";
        }
    }
}
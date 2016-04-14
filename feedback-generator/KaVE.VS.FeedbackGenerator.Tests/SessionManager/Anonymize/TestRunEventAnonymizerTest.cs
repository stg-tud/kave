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

using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils.Collections;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize
{
    internal class TestRunEventAnonymizerTest : IDEEventAnonymizerTestBase<TestRunEvent>
    {
        protected override TestRunEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new TestRunEvent
            {
                Tests =
                {
                    new TestCaseResult
                    {
                        TestMethod = MethodName.Get("[T,P] [T,P].M()"),
                        Parameters = "abc"
                    }
                }
            };
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(TestRunEvent original,
            TestRunEvent anonymized)
        {
            var expecteds = Sets.NewHashSet(
                new TestCaseResult
                {
                    TestMethod = MethodName.Get("[T,P] [T,P].M()").ToAnonymousName(),
                    Parameters = "abc".ToHash()
                });
            Assert.AreEqual(expecteds, anonymized.Tests);
        }
    }
}
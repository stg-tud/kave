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
 */

using KaVE.Model.Events;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
{
    [TestFixture]
    internal class ErrorEventAnonymizerTest : IDEEventAnonymizerTestBase<ErrorEvent>
    {
        protected override ErrorEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new ErrorEvent
            {
                StackTrace = new[]
                {
                    "System.Exception : Exception of type 'System.Exception' was thrown.",
                    " at KaVE...ErrorEventAnonymizerTest.AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(ErrorEvent original, ErrorEvent anonymized) in ErrorEventAnonymizerTest.cs: line 60",
                    " at KaVE...IDEEventAnonymizerTestBase`1.ShouldNotTouchFieldsThatDontNeedToBeAnonymized() in IDEEventAnonymizerTestBase.cs: line 176"
                },
                Content = "Some potentially private payload"
            };
        }

        [Test]
        public void ShouldRemoveContentWhenRemoveNamesIsSet()
        {
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.Content);
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(ErrorEvent original,
            ErrorEvent anonymized)
        {
            Assert.AreEqual(original.StackTrace, anonymized.StackTrace);
        }
    }
}
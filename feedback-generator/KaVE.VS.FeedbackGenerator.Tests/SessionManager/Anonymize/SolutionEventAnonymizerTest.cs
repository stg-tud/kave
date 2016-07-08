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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize
{
    internal class SolutionEventAnonymizerTest : IDEEventAnonymizerTestBase<SolutionEvent>
    {
        protected override SolutionEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.OpenSolution,
                Target = Names.Solution("C:\\Solution.sln")
            };
        }

        [Test]
        public void ShouldRemovePathFromTargetSolutionIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = Names.Solution("\\Solution.sln");
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(Names.Solution("IIjG4xFiAr-N5Azsooms7Q=="), actual.Target);
        }

        [Test]
        public void ShouldRemovePathFromTargetProjectIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = Names.Project("Folder \\A\\B\\C");
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(Names.Project("Folder PzkZVbprHnmonKxi9BX_dg=="), actual.Target);
        }

        [Test]
        public void ShouldRemovePathFromTargetProjectItemIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = Names.ProjectItem("CSharp \\A\\B\\Class.cs");
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(Names.ProjectItem("CSharp YcTmIkbaoyTmRIAE4DoySg=="), actual.Target);
        }


        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(SolutionEvent original,
            SolutionEvent anonymized)
        {
            Assert.AreEqual(original.Action, anonymized.Action);
        }
    }
}
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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
{
    [TestFixture]
    internal class SolutionEventAnonymizerTest : IDEEventAnonymizerTestBase<SolutionEvent>
    {
        protected override SolutionEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.OpenSolution,
                Target = SolutionName.Get("C:\\Solution.sln")
            };
        }

        [Test]
        public void ShouldRemovePathFromTargetSolutionIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = SolutionName.Get("\\Solution.sln");
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(SolutionName.Get("IIjG4xFiAr-N5Azsooms7Q=="), actual.Target);
        }

        [Test]
        public void ShouldRemovePathFromTargetProjectIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = ProjectName.Get("Folder \\A\\B\\C");
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(ProjectName.Get("Folder PzkZVbprHnmonKxi9BX_dg=="), actual.Target);
        }

        [Test]
        public void ShouldRemovePathFromTargetProjectItemIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = ProjectItemName.Get("CSharp \\A\\B\\Class.cs");
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(ProjectItemName.Get("CSharp YcTmIkbaoyTmRIAE4DoySg=="), actual.Target);
        }


        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(SolutionEvent original,
            SolutionEvent anonymized)
        {
            Assert.AreEqual(original.Action, anonymized.Action);
        }
    }
}
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

using KaVE.Model.Events.VisualStudio;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.Utils.Names;
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
            OriginalEvent.Target = SolutionName.Get("C:\\Solution.sln");
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(SolutionName.Get("H/MB2iBprhCn9SyXdxnVNQ=="), actual.Target);
        }

        [Test]
        public void ShouldRemovePathFromTargetProjectIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = ProjectName.Get("Folder C:\\A\\B\\C");
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(ProjectName.Get("Folder IklTG/YtPBAhWOIrB65I1Q=="), actual.Target);
        }

        [Test]
        public void ShouldRemovePathFromTargetProjectItemIfRemoveNamesIsSet()
        {
            OriginalEvent.Target = ProjectItemName.Get("CSharp C:\\A\\B\\Class.cs");
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(ProjectItemName.Get("CSharp nmTd/+pgymTyNZrw5bGrpg=="), actual.Target);
        }


        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(SolutionEvent original,
            SolutionEvent anonymized)
        {
            Assert.AreEqual(original.Action, anonymized.Action);
        }
    }
}
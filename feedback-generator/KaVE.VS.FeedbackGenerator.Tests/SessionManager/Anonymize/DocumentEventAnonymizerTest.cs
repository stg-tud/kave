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
using KaVE.VS.FeedbackGenerator.Utils.Names;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize
{
    internal class DocumentEventAnonymizerTest : IDEEventAnonymizerTestBase<DocumentEvent>
    {
        protected override DocumentEvent CreateEventWithAllAnonymizablePropertiesSet()
        {
            return new DocumentEvent
            {
                Action = DocumentEvent.DocumentAction.Saved,
                Document = VsComponentNameFactory.GetDocumentName("CSharp", "D:\\MyProject\\MyDocument.ext")
            };
        }

        [Test]
        public void ShouldAnonymizeDocumentNameIfRemoveNamesIsSet()
        {
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual("CSharp 8N6R62epS9uMMPQ7P4-rNA==", actual.Document.Identifier);
        }

        protected override void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(DocumentEvent original,
            DocumentEvent anonymized)
        {
            Assert.AreEqual(original.Action, anonymized.Action);
        }
    }
}
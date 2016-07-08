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

using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    internal class DocumentNameCollectorTest
    {
        private DocumentNameCollector _uut;

        [SetUp]
        public void CreateCollector()
        {
            _uut = new DocumentNameCollector();
        }

        [Test]
        public void CollectsNameFromDocumentEvent()
        {
            var windowEvent = new DocumentEvent {Document = Names.Document("document event")};

            Process(windowEvent);

            AssertNameCollected(windowEvent.Document);
        }

        [Test]
        public void CollectsActiveDocument()
        {
            var ideEvent = new TestIDEEvent {ActiveDocument = Names.Document("active document name")};

            Process(ideEvent);

            AssertNameCollected(ideEvent.ActiveDocument);
        }

        [Test]
        public void CollectsOpenDocumentsFromIDEStateEvent()
        {
            var ideEvent = new IDEStateEvent {OpenDocuments = {Names.Document("doc 1"), Names.Document("doc 2")}};

            Process(ideEvent);

            AssertNameCollected(ideEvent.OpenDocuments.ToArray());
        }

        private void Process(IDEEvent @event)
        {
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(@event);
            _uut.OnStreamEnds();
        }

        private void AssertNameCollected(params IDocumentName[] expected)
        {
            var actuals = _uut.AllDocumentNames;
            CollectionAssert.IsSubsetOf(expected.Select(dn => dn.Identifier), actuals);
        }
    }
}
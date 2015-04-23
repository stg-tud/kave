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
 *    - Mattis Manfred Kämmerer
 *    - Markus Zimmermann
 */

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    internal class AddFileProcessorTest
    {
        private AddFileProcessor _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new AddFileProcessor();
        }

        [Test]
        public void ShouldNotFilterAnySolutionEvents()
        {
            ProcessorAssert.DoesNotFilter(new SolutionEvent(), _uut);
        }

        [Test]
        public void ShouldNotFilterAnyDocumentEvents()
        {
            ProcessorAssert.DoesNotFilter(new DocumentEvent(), _uut);
        }

        [Test]
        public void ShouldNotFilterAnyOtherEvents()
        {
            ProcessorAssert.DoesNotFilter(new TestIDEEvent(), _uut);
        }

        [TestCase("CSharp Test.cs", "AddNewItem"), 
         TestCase("CSharp Foo.cpp", "Project.AddClass"),
         TestCase("CSharp Foo.c", "Class")]
        public void GeneratesNewAddFileEventTest(string testIdentifier, string commandId)
        {
            var addNewItemCommandEvent = new CommandEvent {CommandId = commandId};
            var documentEvent = new DocumentEvent
            {
                Document = DocumentName.Get(testIdentifier)
            };

            var solutionEvent = ProcessCommandAndDocumentEvent(addNewItemCommandEvent, documentEvent);

            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(documentEvent.Document, solutionEvent.Target);
            Assert.AreEqual(SolutionEvent.SolutionAction.AddProjectItem, solutionEvent.Action);
        }

        private SolutionEvent ProcessCommandAndDocumentEvent(CommandEvent addNewItemCommandEvent, DocumentEvent documentEvent)
        {
            _uut.Process(addNewItemCommandEvent);
            var processedEvent = _uut.Process(documentEvent);

            Assert.IsInstanceOf<SolutionEvent>(processedEvent);
            return processedEvent as SolutionEvent;
        }
    }
}
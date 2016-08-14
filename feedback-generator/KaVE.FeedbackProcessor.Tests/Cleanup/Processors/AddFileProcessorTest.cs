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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
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
            ProcessorAssert.DoesNotFilter(_uut, new SolutionEvent());
        }

        [Test]
        public void ShouldNotFilterAnyDocumentEvents()
        {
            ProcessorAssert.DoesNotFilter(_uut, new DocumentEvent());
        }

        [Test]
        public void ShouldNotFilterAnyOtherEvents()
        {
            ProcessorAssert.DoesNotFilter(_uut, new TestIDEEvent());
        }

        [TestCase("AddNewItem"),
         TestCase("Project.AddClass"),
         TestCase("Class")]
        public void ShouldNotFilterAddNewItemCommandEvent(string commandId)
        {
            ProcessorAssert.DoesNotFilter(_uut, new CommandEvent {CommandId = commandId});
        }

        [TestCase("CSharp Test.cs", "AddNewItem"),
         TestCase("CSharp Foo.cpp", "Project.AddClass"),
         TestCase("CSharp Foo.c", "Class")]
        public void GenerateSolutionEventTest(string documentIdentifier, string commandId)
        {
            var addNewItemCommandEvent = new CommandEvent {CommandId = commandId};
            var documentEvent = new DocumentEvent
            {
                Document = Names.Document(documentIdentifier)
            };

            _uut.Map(addNewItemCommandEvent);
            var actuals = _uut.Map(documentEvent);

            var expectedSolutionEvent = new SolutionEvent
            {
                Target = documentEvent.Document,
                Action = SolutionAction.AddProjectItem
            };
            CollectionAssert.AreEquivalent(new IDEEvent[] {documentEvent, expectedSolutionEvent}, actuals);
        }

        [Test]
        public void ResetsForNewDeveloper()
        {
            var addNewItemCommandEvent = new CommandEvent {CommandId = "AddNewItem"};
            var documentEvent = new DocumentEvent {Document = Names.Document("CSharp Test.cs")};

            _uut.Map(addNewItemCommandEvent);
            _uut.OnStreamEnds();
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            var actuals = _uut.Map(documentEvent);

            CollectionAssert.AreEqual(new[] {documentEvent}, actuals);
        }
    }
}
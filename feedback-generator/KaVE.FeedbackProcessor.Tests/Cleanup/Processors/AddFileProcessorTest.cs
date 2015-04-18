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
            var solutionEvent = new SolutionEvent();

            var processedEvent = _uut.Process(solutionEvent);

            Assert.AreEqual(solutionEvent, processedEvent);
        }

        [Test]
        public void ShouldNotFilterAnyDocumentEvents()
        {
            var documentEvent = new DocumentEvent();

            var processedEvent = _uut.Process(documentEvent);

            Assert.AreEqual(documentEvent, processedEvent);
        }

        [Test]
        public void ShouldNotFilterAnyOtherEvents()
        {
            var anyEvent = new TestIDEEvent();

            var processedEvent = _uut.Process(anyEvent);

            Assert.AreEqual(anyEvent, processedEvent);
        }

        [Test]
        public void GeneratesNewAddFileEventForAddNewItemCommand()
        {
            const string testIdentifier = "CSharp Test.cs";

            var addNewItemCommandEvent = new CommandEvent {CommandId = "AddNewItem"};
            var documentEvent = new DocumentEvent
            {
                Document = DocumentName.Get(string.Format(testIdentifier))
            };

            _uut.Process(addNewItemCommandEvent);
            var processedEvent = _uut.Process(documentEvent);

            Assert.IsInstanceOf<SolutionEvent>(processedEvent);
            var solutionEvent = processedEvent as SolutionEvent;

            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(testIdentifier, solutionEvent.Target.Identifier);
            Assert.AreEqual(SolutionEvent.SolutionAction.AddProjectItem, solutionEvent.Action);
        }

        [Test]
        public void GeneratesNewAddFileEventForAddClassEvent()
        {
            const string testIdentifier = "CSharp Foo.cpp";

            var addNewItemCommandEvent = new CommandEvent {CommandId = "Project.AddClass"};
            var documentEvent = new DocumentEvent
            {
                Document = DocumentName.Get(string.Format(testIdentifier))
            };

            _uut.Process(addNewItemCommandEvent);
            var processedEvent = _uut.Process(documentEvent);

            Assert.IsInstanceOf<SolutionEvent>(processedEvent);
            var solutionEvent = processedEvent as SolutionEvent;

            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(testIdentifier, solutionEvent.Target.Identifier);
            Assert.AreEqual(SolutionEvent.SolutionAction.AddProjectItem, solutionEvent.Action);
        }

        [Test]
        public void GeneratesNewAddFileEventForClassEvent()
        {
            const string testIdentifier = "CSharp Foo.c";

            var addNewItemCommandEvent = new CommandEvent {CommandId = "Class"};
            var documentEvent = new DocumentEvent
            {
                Document = DocumentName.Get(string.Format(testIdentifier))
            };

            _uut.Process(addNewItemCommandEvent);
            var processedEvent = _uut.Process(documentEvent);

            Assert.IsInstanceOf<SolutionEvent>(processedEvent);
            var solutionEvent = processedEvent as SolutionEvent;

            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(testIdentifier, solutionEvent.Target.Identifier);
            Assert.AreEqual(SolutionEvent.SolutionAction.AddProjectItem, solutionEvent.Action);
        }
    }
}
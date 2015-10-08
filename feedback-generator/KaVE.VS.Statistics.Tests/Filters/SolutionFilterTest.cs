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
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.VS.Statistics.Filters;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Filters
{
    [TestFixture]
    internal class SolutionFilterTest
    {
        [SetUp]
        public void Init()
        {
            _solutionPreprocessor = new SolutionPreprocessor();
        }

        private SolutionPreprocessor _solutionPreprocessor;

        private static readonly SolutionEvent AddProjectEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.AddProject
        };

        private static readonly SolutionEvent AddProjectItemEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.AddProjectItem
        };

        private static readonly SolutionEvent AddSolutionItemEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.AddSolutionItem
        };

        private static readonly SolutionEvent CloseSolutionEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.CloseSolution
        };

        private static readonly SolutionEvent OpenSolutionEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.OpenSolution
        };

        private static readonly SolutionEvent RemoveProjectEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.RemoveProject
        };

        private static readonly SolutionEvent RemoveProjectItemEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.RemoveProjectItem
        };

        private static readonly SolutionEvent RemoveSolutionEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.RemoveSolutionItem
        };

        private static readonly SolutionEvent RenameProjectEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.RenameProject
        };

        private static readonly SolutionEvent RenameProjectItemEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.RenameProjectItem
        };

        private static readonly SolutionEvent RenameSolutionEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.RenameSolution
        };

        private static readonly SolutionEvent RenameSolutionItemEvent = new SolutionEvent
        {
            Action = SolutionEvent.SolutionAction.RenameSolutionItem
        };

        private static readonly SolutionEvent[][] SolutionEvents =
        {
            new[] {AddProjectEvent, AddProjectEvent},
            new[] {AddProjectItemEvent, AddProjectItemEvent},
            new[] {AddSolutionItemEvent, AddSolutionItemEvent},
            new[] {CloseSolutionEvent, CloseSolutionEvent},
            new[] {OpenSolutionEvent, OpenSolutionEvent},
            new[] {RemoveProjectEvent, RemoveProjectEvent},
            new[] {RemoveProjectItemEvent, RemoveProjectItemEvent},
            new[] {RemoveSolutionEvent, RemoveSolutionEvent},
            new[] {RenameProjectEvent, RenameProjectEvent},
            new[] {RenameProjectItemEvent, RenameProjectItemEvent},
            new[] {RenameSolutionEvent, RenameSolutionEvent},
            new[] {RenameSolutionItemEvent, RenameSolutionItemEvent}
        };

        public static readonly IDEEvent[] NonSolutionEvents =
        {
            new CommandEvent(),
            new DocumentEvent()
        };

        private static readonly object[][] AddedTestClassTestSources =
        {
            new object[]
            {
                new DocumentEvent {Document = DocumentName.Get("CSharp Test.cs")},
                new CommandEvent {CommandId = "AddNewItem"},
                "Test.cs"
            },
            new object[]
            {
                new DocumentEvent {Document = DocumentName.Get("CSharp Foo.cpp")},
                new CommandEvent {CommandId = "Project.AddClass"},
                "Foo.cpp"
            }
        };

        [Test, TestCaseSource("NonSolutionEvents")]
        public void FiltersEventsCorrectly(IDEEvent @event)
        {
            Assert.IsNull(_solutionPreprocessor.Preprocess(@event));
        }

        [Test, TestCaseSource("AddedTestClassTestSources")]
        public void ForwardsAddedItemEventTest(DocumentEvent documentEvent,
            CommandEvent commandEvent,
            string expectedIdentifier)
        {
            _solutionPreprocessor.Preprocess(documentEvent);
            var actualEvent = (SolutionEvent) _solutionPreprocessor.Preprocess(commandEvent);

            const SolutionEvent.SolutionAction expectedAction = SolutionEvent.SolutionAction.AddProjectItem;
            Assert.AreEqual(expectedAction, actualEvent.Action);
        }

        [Test, TestCaseSource("SolutionEvents")]
        public void ProcessesSolutionEventsCorrectly(IDEEvent publishedEvent, IDEEvent expectedEvent)
        {
            var actualEvent = _solutionPreprocessor.Preprocess(publishedEvent);

            Assert.AreEqual(expectedEvent, actualEvent);
        }
    }
}
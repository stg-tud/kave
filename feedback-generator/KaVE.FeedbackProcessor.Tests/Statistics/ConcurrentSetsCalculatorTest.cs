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

using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Statistics;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class ConcurrentSetsCalculatorTest
    {
        private ConcurrentSetsCalculator _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new ConcurrentSetsCalculator();
        }

        private static readonly object[] MapsSingleEventCorrectlyTestCaseSource =
        {
            new object[] {new CommandEvent {CommandId = "Test"}, "Command -> Test"},
            new object[] {new WindowEvent {Action = WindowEvent.WindowAction.Deactivate}, "Window -> Deactivate"},
            new object[] {new DocumentEvent {Action = DocumentEvent.DocumentAction.Opened}, "Document -> Opened"},
            new object[] {new BuildEvent {Action = "vsBuildActionBuild"}, "Build -> vsBuildActionBuild"},
            new object[] {new EditEvent {NumberOfChanges = 1984}, "Edit -> 1984 Changes"},
            new object[]
            {
                new DebuggerEvent {Reason = "dbgEventReasonStopDebugging"},
                "Debugger -> dbgEventReasonStopDebugging"
            },
            new object[]
            {
                new IDEStateEvent {IDELifecyclePhase = IDEStateEvent.LifecyclePhase.Shutdown},
                "IDEState -> Shutdown"
            },
            new object[]
            {
                new SolutionEvent {Action = SolutionEvent.SolutionAction.RenameProject},
                "Solution -> RenameProject"
            },
            new object[]
            {
                new CompletionEvent {TerminatedState = TerminationState.Unknown},
                "Completion -> Terminated as Unknown"
            },
            new object[]
            {
                new ErrorEvent{StackTrace = new []{"System.NullReferenceException: Test"}},
                "Error -> System.NullReferenceException"
            } 
        };

        [Test, TestCaseSource("MapsSingleEventCorrectlyTestCaseSource")]
        public void MapsSingleEventCorrectly(IDEEvent inputEvent, string expectedString)
        {
            var concurrentEvent = new ConcurrentEvent
            {
                ConcurrentEventList = new List<IDEEvent> {inputEvent}
            };

            _uut.OnEvent(concurrentEvent);

            CollectionAssert.Contains(
                _uut.Statistic,
                new KeyValuePair<ISet<string>, int>(Sets.NewHashSet(expectedString), 1));
        }

        [Test]
        public void CountsEqualSetsCorrectly()
        {
            var concurrentEvent = new ConcurrentEvent
            {
                ConcurrentEventList = new List<IDEEvent>
                {
                    new CommandEvent {CommandId = "Test"},
                    new DocumentEvent {Action = DocumentEvent.DocumentAction.Saved}
                }
            };

            _uut.OnEvent(concurrentEvent);
            _uut.OnEvent(concurrentEvent);

            Assert.AreEqual(2, _uut.Statistic[Sets.NewHashSet("Document -> Saved", "Command -> Test")]);
        }

        [Test]
        public void ShouldIgnoreAllOtherEvents()
        {
            _uut.OnEvent(IDEEventTestFactory.SomeEvent());

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void ShouldNotAddEmptySetToStatistic()
        {
            var concurrentEvent = new ConcurrentEvent
            {
                ConcurrentEventList = IDEEventTestFactory.SomeEvents(5)
            };

            _uut.OnEvent(concurrentEvent);

            CollectionAssert.IsEmpty(_uut.Statistic);
        }
    }
}
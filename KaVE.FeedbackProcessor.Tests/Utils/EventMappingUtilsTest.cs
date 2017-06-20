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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Utils
{
    internal class EventMappingUtilsTest
    {
        private static readonly object[] MapsSingleEventCorrectlyTestCaseSource =
        {
            new object[] {new CommandEvent {CommandId = "Test"}, "CommandEvent -> Test"},
            new object[] {new WindowEvent {Action = WindowAction.Deactivate}, "WindowEvent -> Deactivate"},
            new object[] {new DocumentEvent {Action = DocumentAction.Opened}, "DocumentEvent -> Opened"},
            new object[] {new BuildEvent {Action = "vsBuildActionBuild"}, "BuildEvent -> vsBuildActionBuild"},
            new object[] {new EditEvent(), "EditEvent -> "},
            new object[]
            {
                new DebuggerEvent {Reason = "dbgEventReasonStopDebugging"},
                "DebuggerEvent -> dbgEventReasonStopDebugging"
            },
            new object[]
            {
                new IDEStateEvent {IDELifecyclePhase = IDELifecyclePhase.Shutdown},
                "IDEStateEvent -> Shutdown"
            },
            new object[]
            {
                new SolutionEvent {Action = SolutionAction.RenameProject},
                "SolutionEvent -> RenameProject"
            },
            new object[]
            {
                new CompletionEvent {TerminatedState = TerminationState.Unknown},
                "CompletionEvent -> Terminated as Unknown"
            },
            new object[]
            {
                new ErrorEvent {StackTrace = new[] {"System.NullReferenceException: Test"}},
                "ErrorEvent -> System.NullReferenceException"
            }
        };

        [Test, TestCaseSource("MapsSingleEventCorrectlyTestCaseSource")]
        public void ShouldMapToAbstractStringCorrectly(IDEEvent inputEvent, string expectedString)
        {
            Assert.AreEqual(expectedString, EventMappingUtils.GetAbstractStringOf(inputEvent));
        }

        public void ShouldMapToSpecialStringForTypesWithoutMapping()
        {
            Assert.AreEqual(
                "TestIDEEvent -> no mapping found",
                EventMappingUtils.GetAbstractStringOf(TestEventFactory.SomeEvent()));
        }
    }
}
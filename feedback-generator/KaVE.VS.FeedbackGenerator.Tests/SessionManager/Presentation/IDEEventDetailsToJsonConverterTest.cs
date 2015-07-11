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

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Presentation
{
    internal class IDEEventDetailsToJsonConverterTest
    {
        [Test]
        public void ShouldConvertActionEventDetailsToXaml()
        {
            var actionEvent = new WindowEvent
            {
                Window = WindowName.Get("MyWindow"),
                Action = WindowEvent.WindowAction.Create
            };
            string expected = String.Join(
                Environment.NewLine,
                "    \"Window\": \"MyWindow\"",
                "    \"Action\": \"Create\"");
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldConvertCompletionEventWhileHidingProperties()
        {
            var completionEvent = new CompletionEvent
            {
                Context2 = new Context {SST = new SST {EnclosingType = TypeName.Get("TestClass,TestProject")}},
                ProposalCollection =
                {
                    new Proposal {Name = FieldName.Get("[FieldType,P] [TestClass,P].SomeField")},
                    new Proposal {Name = EventName.Get("[EventType`1[[T -> EventArgsType,P]],P] [DeclaringType,P].E")},
                    new Proposal {Name = MethodName.Get("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")}
                },
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal {Name = FieldName.Get("[FieldType,P] [TestClass,P].SomeField")},
                        SelectedAfter = TimeSpan.FromSeconds(1)
                    },
                    new ProposalSelection
                    {
                        Proposal =
                            new Proposal
                            {
                                Name = EventName.Get("[EventType`1[[T -> EventArgsType,P]],P] [DeclaringType,P].E")
                            },
                        SelectedAfter = TimeSpan.FromSeconds(2)
                    },
                    new ProposalSelection
                    {
                        Proposal =
                            new Proposal
                            {
                                Name = MethodName.Get("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")
                            },
                        SelectedAfter = TimeSpan.FromSeconds(3)
                    }
                },
                Prefix = "Some",
                TerminatedBy = IDEEvent.Trigger.Typing,
                TerminatedState = TerminationState.Cancelled
            };

            var expected = String.Join(
                Environment.NewLine,
                "    \"Prefix\": \"Some\"",
                "    \"TerminatedBy\": \"Typing\"",
                "    \"TerminatedState\": \"Cancelled\"");
            var actual = completionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotIncludeIDEEventPropertiesInDetails()
        {
            var actionEvent = new CommandEvent
            {
                ActiveDocument = DocumentName.Get("Doc"),
                ActiveWindow = WindowName.Get("Window"),
                IDESessionUUID = "UUID",
                TerminatedAt = DateTime.Now,
                TriggeredAt = DateTime.Now,
                TriggeredBy = IDEEvent.Trigger.Click
            };
            const string expected = "";
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }
    }
}
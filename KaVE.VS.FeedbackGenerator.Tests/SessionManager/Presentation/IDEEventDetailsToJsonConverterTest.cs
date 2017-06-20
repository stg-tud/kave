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
using KaVE.Commons.Model.Naming;
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
                Window = Names.Window("w MyWindow"),
                Action = WindowAction.Create
            };
            var expected = string.Join(
                Environment.NewLine,
                "    \"Window\": \"w MyWindow\"",
                "    \"Action\": \"Create\"");
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldConvertCompletionEventWhileHidingProperties()
        {
            var completionEvent = new CompletionEvent
            {
                Context2 = new Context {SST = new SST {EnclosingType = Names.Type("TestClass,TestProject")}},
                ProposalCollection =
                {
                    new Proposal {Name = Names.Field("[FieldType,P] [TestClass,P].SomeField")},
                    new Proposal {Name = Names.Event("[EventType`1[[T -> EventArgsType,P]],P] [DeclaringType,P].E")},
                    new Proposal {Name = Names.Method("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")}
                },
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal {Name = Names.Field("[FieldType,P] [TestClass,P].SomeField")},
                        SelectedAfter = TimeSpan.FromSeconds(1)
                    },
                    new ProposalSelection
                    {
                        Proposal =
                            new Proposal
                            {
                                Name = Names.Event("[EventType`1[[T -> EventArgsType,P]],P] [DeclaringType,P].E")
                            },
                        SelectedAfter = TimeSpan.FromSeconds(2)
                    },
                    new ProposalSelection
                    {
                        Proposal =
                            new Proposal
                            {
                                Name = Names.Method("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")
                            },
                        SelectedAfter = TimeSpan.FromSeconds(3)
                    }
                },
                TerminatedBy = EventTrigger.Typing,
                TerminatedState = TerminationState.Cancelled,
                ProposalCount = 1
            };

            var expected = string.Join(
                Environment.NewLine,
                "    \"TerminatedBy\": \"Typing\"",
                "    \"TerminatedState\": \"Cancelled\"",
                "    \"ProposalCount\": 1");
            var actual = completionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldFormatEditEventDetailsWhileHidingContext()
        {
            var editEvent = new EditEvent
            {
                Context2 = new Context {SST = new SST {EnclosingType = Names.Type("TestClass,TestProject")}},
                NumberOfChanges = 2,
                SizeOfChanges = 20
            };

            var expected = String.Join(
                Environment.NewLine,
                "    \"NumberOfChanges\": 2",
                "    \"SizeOfChanges\": 20");
            var actual = editEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotIncludeIDEEventPropertiesInDetails()
        {
            var actionEvent = new CommandEvent
            {
                ActiveDocument = Names.Document("d Doc"),
                ActiveWindow = Names.Window("w Window"),
                IDESessionUUID = "UUID",
                TerminatedAt = DateTime.Now,
                TriggeredAt = DateTime.Now,
                TriggeredBy = EventTrigger.Click
            };
            const string expected = "";
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }
    }
}
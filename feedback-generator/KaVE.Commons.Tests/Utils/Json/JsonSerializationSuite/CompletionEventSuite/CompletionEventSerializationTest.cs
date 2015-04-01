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
 *    - Sven Amann
 *    - Dennis Albrecht
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    internal class CompletionEventSerializationTest : SerializationTestBase
    {
        [Test]
        public void VerifyToJson()
        {
            var actual = GetExample().ToCompactJson();
            var expected = GetExampleJson_Current();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromJson()
        {
            var actual = GetExampleJson_Current().ParseJsonTo<CompletionEvent>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromJson_Legacy_BeforeRenamingTheCodeCompletionNamespace()
        {
            var actual = GetExampleJson_Legacy_BeforeRenamingTheCodeCompletionNamespace().ParseJsonTo<CompletionEvent>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromJson_Legacy_BeforeRestructuringProjects()
        {
            var actual = GetExampleJson_Legacy_BeforeRestructuringProjects().ParseJsonTo<CompletionEvent>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyObjToObjEquality()
        {
            var actual = GetExample().ToCompactJson().ParseJsonTo<CompletionEvent>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        private static CompletionEvent GetExample()
        {
            var now = new System.DateTime(2012, 2, 23, 18, 54, 59, 549);
            return new CompletionEvent
            {
                /* IDEEvent */
                IDESessionUUID = "0xDEADBEEF",
                KaVEVersion = "1.0",
                TriggeredAt = now,
                TriggeredBy = IDEEvent.Trigger.Unknown,
                Duration = TimeSpan.FromSeconds(2),
                ActiveWindow = WindowName.Get("vsWindowTypeDocument File.cs"),
                ActiveDocument = DocumentName.Get("\\Path\\To\\File.cs"),
                /* CompletionEvent */
                Context2 = new Context {SST = new SST {EnclosingType = TypeName.Get("T,P")}},
                ProposalCollection =
                {
                    Proposals =
                    {
                        new Proposal
                        {
                            Name = MethodName.Get("[T1,P1] [T1,P2].M1()"),
                            Relevance = 42
                        },
                        new Proposal
                        {
                            Name = MethodName.Get("[T1,P1] [T1,P2].M2()"),
                            Relevance = 42
                        }
                    }
                },
                Prefix = "Foo",
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = MethodName.Get("[T1,P1] [T1,P2].M1()"),
                            Relevance = 42
                        },
                        SelectedAfter = now.AddMilliseconds(123).TimeOfDay
                    },
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = MethodName.Get("[T1,P1] [T1,P2].M2()"),
                            Relevance = 42
                        },
                        SelectedAfter = now.AddMilliseconds(234).TimeOfDay
                    },
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = MethodName.Get("[T1,P1] [T1,P2].M1()"),
                            Relevance = 42
                        },
                        SelectedAfter = now.AddMilliseconds(345).TimeOfDay
                    },
                },
                TerminatedBy = IDEEvent.Trigger.Typing,
                TerminatedState = TerminationState.Applied
            };
        }

        private static string GetExampleJson_Legacy_BeforeRenamingTheCodeCompletionNamespace()
        {
            // do not change! keep for checking exception free reading of old formats!
            return
                "{\"$type\":\"KaVE.Model.Events.CompletionEvent.CompletionEvent, KaVE.Model\",\"Context2\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.Context, KaVE.Model\",\"TypeShape\":{\"$type\":\"KaVE.Model.TypeShapes.TypeShape, KaVE.Model\",\"TypeHierarchy\":{\"$type\":\"KaVE.Model.TypeShapes.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.UnknownTypeName:?\",\"Implements\":[]},\"MethodHierarchies\":[]},\"SST\":{\"$type\":\"KaVE.Model.SSTs.Impl.SST, KaVE.Model\",\"EnclosingType\":\"CSharp.TypeName:T,P\",\"Fields\":[],\"Properties\":[],\"Methods\":[],\"Events\":[],\"Delegates\":[]}},\"ProposalCollection\":[{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M2()\",\"Relevance\":42}],\"Prefix\":\"Foo\",\"Selections\":[{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model\",\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.6720000\"},{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model\",\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M2()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.7830000\"},{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model\",\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.8940000\"}],\"TerminatedBy\":3,\"TerminatedState\":0,\"IDESessionUUID\":\"0xDEADBEEF\",\"KaVEVersion\":\"1.0\",\"TriggeredAt\":\"2012-02-23T18:54:59.549\",\"TriggeredBy\":0,\"Duration\":\"00:00:02\",\"ActiveWindow\":\"VisualStudio.WindowName:vsWindowTypeDocument File.cs\",\"ActiveDocument\":\"VisualStudio.DocumentName:\\\\Path\\\\To\\\\File.cs\"}";
        }

        private static string GetExampleJson_Legacy_BeforeRestructuringProjects()
        {
            // should reflect current serialization format!
            return
                "{\"$type\":\"KaVE.Model.Events.CompletionEvents.CompletionEvent, KaVE.Model\",\"Context2\":{\"$type\":\"KaVE.Model.Events.CompletionEvents.Context, KaVE.Model\",\"TypeShape\":{\"$type\":\"KaVE.Model.TypeShapes.TypeShape, KaVE.Model\",\"TypeHierarchy\":{\"$type\":\"KaVE.Model.TypeShapes.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.UnknownTypeName:?\",\"Implements\":[]},\"MethodHierarchies\":[]},\"SST\":{\"$type\":\"KaVE.Model.SSTs.Impl.SST, KaVE.Model\",\"EnclosingType\":\"CSharp.TypeName:T,P\",\"Fields\":[],\"Properties\":[],\"Methods\":[],\"Events\":[],\"Delegates\":[]}},\"ProposalCollection\":[{\"$type\":\"KaVE.Model.Events.CompletionEvents.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},{\"$type\":\"KaVE.Model.Events.CompletionEvents.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M2()\",\"Relevance\":42}],\"Prefix\":\"Foo\",\"Selections\":[{\"$type\":\"KaVE.Model.Events.CompletionEvents.ProposalSelection, KaVE.Model\",\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvents.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.6720000\"},{\"$type\":\"KaVE.Model.Events.CompletionEvents.ProposalSelection, KaVE.Model\",\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvents.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M2()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.7830000\"},{\"$type\":\"KaVE.Model.Events.CompletionEvents.ProposalSelection, KaVE.Model\",\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvents.Proposal, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.8940000\"}],\"TerminatedBy\":3,\"TerminatedState\":0,\"IDESessionUUID\":\"0xDEADBEEF\",\"KaVEVersion\":\"1.0\",\"TriggeredAt\":\"2012-02-23T18:54:59.549\",\"TriggeredBy\":0,\"Duration\":\"00:00:02\",\"ActiveWindow\":\"VisualStudio.WindowName:vsWindowTypeDocument File.cs\",\"ActiveDocument\":\"VisualStudio.DocumentName:\\\\Path\\\\To\\\\File.cs\"}";
        }

        private static string GetExampleJson_Current()
        {
            // should reflect current serialization format!
            return
                "{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.CompletionEvent, KaVE.Commons\",\"Context2\":{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.Context, KaVE.Commons\",\"TypeShape\":{\"$type\":\"KaVE.Commons.Model.TypeShapes.TypeShape, KaVE.Commons\",\"TypeHierarchy\":{\"$type\":\"KaVE.Commons.Model.TypeShapes.TypeHierarchy, KaVE.Commons\",\"Element\":\"CSharp.UnknownTypeName:?\",\"Implements\":[]},\"MethodHierarchies\":[]},\"SST\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.SST, KaVE.Commons\",\"EnclosingType\":\"CSharp.TypeName:T,P\",\"Fields\":[],\"Properties\":[],\"Methods\":[],\"Events\":[],\"Delegates\":[]}},\"ProposalCollection\":[{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.Proposal, KaVE.Commons\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.Proposal, KaVE.Commons\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M2()\",\"Relevance\":42}],\"Prefix\":\"Foo\",\"Selections\":[{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.ProposalSelection, KaVE.Commons\",\"Proposal\":{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.Proposal, KaVE.Commons\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.6720000\"},{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.ProposalSelection, KaVE.Commons\",\"Proposal\":{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.Proposal, KaVE.Commons\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M2()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.7830000\"},{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.ProposalSelection, KaVE.Commons\",\"Proposal\":{\"$type\":\"KaVE.Commons.Model.Events.CompletionEvents.Proposal, KaVE.Commons\",\"Name\":\"CSharp.MethodName:[T1,P1] [T1,P2].M1()\",\"Relevance\":42},\"SelectedAfter\":\"18:54:59.8940000\"}],\"TerminatedBy\":3,\"TerminatedState\":0,\"IDESessionUUID\":\"0xDEADBEEF\",\"KaVEVersion\":\"1.0\",\"TriggeredAt\":\"2012-02-23T18:54:59.549\",\"TriggeredBy\":0,\"Duration\":\"00:00:02\",\"ActiveWindow\":\"VisualStudio.WindowName:vsWindowTypeDocument File.cs\",\"ActiveDocument\":\"VisualStudio.DocumentName:\\\\Path\\\\To\\\\File.cs\"}";
        }
    }
}
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
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.Model.Names.VisualStudio;
using KaVE.Model.SSTs.Impl;
using KaVE.TestUtils.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    internal class CompletionEventSerializationTest : SerializationTestBase
    {
        [Test]
        public void VerifyToJson()
        {
            var actual = GetExample().ToCompactJson();
            var expected = GetExampleJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromJson()
        {
            var actual = GetExampleJson().ParseJsonTo<CompletionEvent>();
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
            var completionEvent = new CompletionEvent
            {
                /* IDEEvent */
                IDESessionUUID = "0xDEADBEEF",
                TriggeredAt = new DateTime(2012, 2, 23, 18, 54, 59, 549),
                TriggeredBy = IDEEvent.Trigger.Unknown,
                TerminatedBy = IDEEvent.Trigger.Typing,
                Duration = TimeSpan.FromSeconds(2),
                ActiveWindow = WindowName.Get("vsWindowTypeDocument File.cs"),
                ActiveDocument = DocumentName.Get("\\Path\\To\\File.cs"),
                /* CompletionEvent */
                ProposalCollection =
                {
                    Proposals =
                    {
                        new Proposal
                        {
                            Name = TestNameFactory.GetAnonymousMethodName(),
                            Relevance = 42
                        },
                        new Proposal
                        {
                            Name = TestNameFactory.GetAnonymousNamespace(),
                            Relevance = -23
                        }
                    }
                },
                Context2 = new Context {SST = new SST {EnclosingType = TypeName.Get("T,P")}},
                Prefix = "Foo",
                TerminatedAt = new DateTime(2012, 2, 23, 18, 54, 59, 549).AddSeconds(5),
                TerminatedState = TerminationState.Applied
            };
            completionEvent.AddSelection(
                new Proposal
                {
                    Name = TestNameFactory.GetAnonymousMethodName(),
                    Relevance = 42
                });
            completionEvent.AddSelection(
                new Proposal
                {
                    Name = TestNameFactory.GetAnonymousNamespace(),
                    Relevance = -23
                });
            completionEvent.AddSelection(
                new Proposal
                {
                    Name = TestNameFactory.GetAnonymousMethodName(),
                    Relevance = 42
                });
            return completionEvent;
        }

        private static string GetExampleJson()
        {
            var jsonEvent = "{\"$type\":\"KaVE.Model.Events.CompletionEvent.CompletionEvent, KaVE.Model\"," +
                            "\"ProposalCollection\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalCollection, KaVE.Model\"," +
                            "\"Proposals\":{\"$type\":\"System.Collections.Generic.List`1[[KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model]], mscorlib\"," +
                            "\"$values\":[" +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanRead()\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanSeek()\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanTimeout()\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanWrite()\"}}," +
                            /* other proposals removed to reduce size of this string */
                            "]}}," +
                            "\"Selections\":{\"$type\":\"System.Collections.Generic.List`1[[KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model]], mscorlib\"," +
                            "\"$values\":[{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model\",\"SelectedAfter\":\"00:00:00\"," +
                            "\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanRead()\"}}}" +
                            "]}," +
                            /* context */
                            "\"Context2\":" + EmptyContextJson() + "," +
                            "\"Prefix\":\"\"," +
                            "\"TerminatedBy\":1," +
                            "\"TerminatedAs\":1," +
                            "\"IDESessionUUID\":\"57d18e20-952f-4583-88b3-3aadc1db48b1\"," +
                            "\"TriggeredAt\":\"2013-12-06T11:34:24\"," +
                            "\"TriggeredBy\":0," +
                            "\"Duration\":\"00:00:02\"," +
                            "\"ActiveWindow\":{\"type\":\"VisualStudio.WindowName\",\"identifier\":\"vsWindowTypeDocument File.cs\"}," +
                            "\"ActiveDocument\":{\"type\":\"VisualStudio.DocumentName\",\"identifier\":\"\\\\Path\\\\To\\\\File.cs\"}}";
            return jsonEvent;
        }

        private static string EmptyContextJson()
        {
            return "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Context, KaVE.Model\"," +
                   "\"TypeShape\":{\"$type\":\"KaVE.Model.TypeShapes.TypeShape, KaVE.Model\"," +
                   "\"TypeHierarchy\":{\"$type\":\"KaVE.Model.TypeShapes.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeNames.UnknownTypeName:?\",\"Implements\":[]}," +
                   "\"MethodHierarchies\":[]}," +
                   "\"SST\":{\"$type\":\"KaVE.Model.SSTs.Impl.SST, KaVE.Model\"," +
                   "\"EnclosingType\":\"CSharp.TypeNames.UnknownTypeName:?\"," +
                   "\"Fields\":[],\"Properties\":[],\"Methods\":[],\"Events\":[],\"Delegates\":[]}}";
        }
    }
}
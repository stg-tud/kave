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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    internal class CompletionEventSerializationTest : SerializationTestBase
    {
        // please note that the tests are migrated to ExternalSerializationTestSuite
        // Use this class as a test data generator in case of changes (set a break point).

        [Test]
        public void SmokeTest()
        {
            var now = new DateTime(2012, 2, 23, 18, 54, 59, 549);
            var sut = new CompletionEvent
            {
                /* IDEEvent */
                IDESessionUUID = "0xDEADBEEF",
                KaVEVersion = "1.0",
                TriggeredAt = now,
                TriggeredBy = EventTrigger.Unknown,
                Duration = TimeSpan.FromSeconds(2),
                ActiveWindow = Names.Window("vsWindowTypeDocument File.cs"),
                ActiveDocument = Names.Document("CSharp \\Path\\To\\File.cs"),
                /* CompletionEvent */
                Context2 = new Context {SST = new SST {EnclosingType = Names.Type("T,P")}},
                ProposalCollection =
                {
                    Proposals =
                    {
                        new Proposal
                        {
                            Name = Names.Method("[T1,P1] [T1,P2].M1()"),
                            Relevance = 42
                        },
                        new Proposal
                        {
                            Name = Names.Method("[T1,P1] [T1,P2].M2()"),
                            Relevance = 42
                        }
                    }
                },
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = Names.Method("[T1,P1] [T1,P2].M1()"),
                            Relevance = 42
                        },
                        SelectedAfter = now.AddMilliseconds(123).TimeOfDay
                    },
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = Names.Method("[T1,P1] [T1,P2].M2()"),
                            Relevance = 42
                        },
                        SelectedAfter = now.AddMilliseconds(234).TimeOfDay
                    },
                    new ProposalSelection
                    {
                        Proposal = new Proposal
                        {
                            Name = Names.Method("[T1,P1] [T1,P2].M1()"),
                            Relevance = 42
                        },
                        SelectedAfter = now.AddMilliseconds(345).TimeOfDay
                    }
                },
                TerminatedBy = EventTrigger.Typing,
                TerminatedState = TerminationState.Applied
            };

            var compact = sut.ToCompactJson();
            var formatted = sut.ToFormattedJson();

            // set breakpoint here and copy string to files
        }
    }
}
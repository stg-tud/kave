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
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.VS.FeedbackGenerator.SessionManager;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class EventViewTest
    {
        [Test]
        public void ShouldDisplayFormattedEventDetails()
        {
            var @event = new CommandEvent {CommandId = "test.command"};
            const string expected =
                "    <Bold>\"CommandId\":</Bold> \"test.command\"";

            var view = new EventViewModel(@event);
            var actual = view.Details;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatsCompletionEventContext()
        {
            var completionEvent = new CompletionEvent
            {
                Context2 = {SST = new SST {EnclosingType = TypeName.Get("TestClass,TestProject")}}
            };

            var view = new EventViewModel(completionEvent);
            Assert.AreEqual(
                "\r\n\r\n<Span Foreground=\"Blue\">class</Span> <Span Foreground=\"#2B91AF\">TestClass</Span>\r\n{\r\n}",
                view.XamlContextRepresentation);
        }

        [Test]
        public void DoesntDisplayContextForOtherEvents()
        {
            var someEvent = IDEEventTestFactory.SomeEvent();

            var view = new EventViewModel(someEvent);
            Assert.IsNull(view.XamlContextRepresentation);
        }

        [Test]
        public void FormatsCompletionEventProposals()
        {
            var completionEvent = new CompletionEvent
            {
                ProposalCollection =
                {
                    new Proposal {Name = FieldName.Get("[FieldType,P] [TestClass,P].SomeField")},
                    new Proposal {Name = EventName.Get("[EventType`1[[T -> EventArgsType,P]],P] [DeclaringType,P].E")},
                    new Proposal {Name = MethodName.Get("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")}
                }
            };

            var view = new EventViewModel(completionEvent);
            Assert.IsNotNullOrEmpty(view.XamlProposalsRepresentation);
        }

        [Test]
        public void DoesntDisplayProposalsForOtherEvents()
        {
            var someEvent = IDEEventTestFactory.SomeEvent();

            var view = new EventViewModel(someEvent);
            Assert.IsNull(view.XamlProposalsRepresentation);
        }

        [Test]
        public void FormatsCompletionEventSelections()
        {
            var completionEvent = new CompletionEvent
            {
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
                }
            };

            var view = new EventViewModel(completionEvent);
            Assert.IsNotNullOrEmpty(view.XamlSelectionsRepresentation);
        }

        [Test]
        public void DoesntFormatSelectionsForOtherEvents()
        {
            var someEvent = IDEEventTestFactory.SomeEvent();

            var view = new EventViewModel(someEvent);
            Assert.IsNull(view.XamlSelectionsRepresentation);
        }

        [Test]
        public void EscapesSpecialCharsInSelections()
        {
            var completionEvent = new CompletionEvent
            {
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal {Name = Name.Get("TypeLookupItem:TestDelegate<string>[]")},
                        SelectedAfter = TimeSpan.FromSeconds(1)
                    }
                }
            };

            var view = new EventViewModel(completionEvent);
            Assert.AreEqual(
                "• <Bold>00:00:01 at index -1</Bold> TypeLookupItem:TestDelegate&lt;string&gt;[]\r\n",
                view.XamlSelectionsRepresentation);
        }

        [Test]
        public void EscapesSpecialCharsInProposals()
        {
            var completionEvent = new CompletionEvent
            {
                ProposalCollection =
                {
                    new Proposal {Name = Name.Get("TypeLookupItem:TestDelegate<string>[]")}
                }
            };

            var view = new EventViewModel(completionEvent);
            Assert.AreEqual("• TypeLookupItem:TestDelegate&lt;string&gt;[]\r\n", view.XamlProposalsRepresentation);
        }

        [Test]
        public void EscapesSpecialCharsInRawView()
        {
            var completionEvent = new CompletionEvent
            {
                ProposalCollection =
                {
                    new Proposal {Name = Name.Get("TypeLookupItem:TestDelegate<string>[]")}
                }
            };

            var view = new EventViewModel(completionEvent);
            StringAssert.Contains("TestDelegate&lt;string&gt;[]", view.XamlRawRepresentation);
        }

        [Test]
        public void EscapesSpecialCharsInLongRawView()
        {
            var completionEvent = new CompletionEvent
            {
                ProposalCollection =
                {
                    new Proposal {Name = Name.Get("TypeLookupItem:TestDelegate<string>[]")},
                    // filler to make the overall serialization longer than 50000 characters
                    new Proposal {Name = Name.Get(new string('a', 50000))}
                }
            };

            var view = new EventViewModel(completionEvent);
            StringAssert.Contains("TestDelegate&lt;string&gt;[]", view.XamlRawRepresentation);
        }

        [Test]
        public void EscapesSpecialCharsInCodeCompletionContext()
        {
            var completionEvent = new CompletionEvent
            {
                Context2 = {SST = new SST {EnclosingType = TypeName.Get("C`1[[T]],TestProject")}}
            };

            var view = new EventViewModel(completionEvent);
            Assert.AreEqual(
                "\r\n\r\n<Span Foreground=\"Blue\">class</Span> <Span Foreground=\"#2B91AF\">C</Span>&lt;<Bold>?</Bold>&gt;\r\n{\r\n}",
                view.XamlContextRepresentation);
        }

        [Test]
        public void DoesntAddHighlightingToEncodedSpecialChars()
        {
            var completionEvent = new CompletionEvent
            {
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal = new Proposal {Name = Name.Get("System.Threading.ThreadLocal`1[[T -> T]], mscorlib, 4.0.0.0")},
                        SelectedAfter = TimeSpan.FromSeconds(1)
                    }
                },
                Prefix = "t"
            };

            var view = new EventViewModel(completionEvent);
            Assert.AreEqual("• <Bold>00:00:01</Bold> Sys<Bold>t</Bold>em.<Bold>T</Bold>hreading.<Bold>T</Bold>hreadLocal`1[[<Bold>T</Bold> -&gt; <Bold>T</Bold>]], mscorlib, 4.0.0.0\r\n", view.XamlSelectionsRepresentation);
        }
    }
}
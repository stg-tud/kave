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
 *    - Sebastian Proksch
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using ComplEvent = KaVE.Commons.Model.Events.CompletionEvents.CompletionEvent;

namespace KaVE.Commons.Tests.Model.Events.CompletionEvent
{
    internal class CompletionEventTest
    {
        private static Context SomeContext
        {
            get { return new Context {SST = new SST {EnclosingType = TypeName.Get("T,P")}}; }
        }

        private static IProposal SomeProposal
        {
            get { return new Proposal {Relevance = 3}; }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new ComplEvent();
            Assert.AreEqual(new Context(), sut.Context2);
            Assert.AreEqual(new ProposalCollection(), sut.ProposalCollection);
            Assert.AreEqual("", sut.Prefix);
            Assert.AreEqual(Lists.NewList<IProposalSelection>(), sut.Selections);
            Assert.AreEqual(IDEEvent.Trigger.Unknown, sut.TerminatedBy);
            Assert.AreEqual(TerminationState.Unknown, sut.TerminatedState);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ComplEvent
            {
                Context2 = SomeContext,
                ProposalCollection = {SomeProposal},
                Prefix = "x",
                Selections = {new ProposalSelection()},
                TerminatedBy = IDEEvent.Trigger.Click,
                TerminatedState = TerminationState.Filtered
            };
            Assert.AreEqual(SomeContext, sut.Context2);
            Assert.AreEqual(new ProposalCollection {SomeProposal}, sut.ProposalCollection);
            Assert.AreEqual("x", sut.Prefix);
            Assert.AreEqual(Lists.NewList(new ProposalSelection()), sut.Selections);
            Assert.AreEqual(IDEEvent.Trigger.Click, sut.TerminatedBy);
            Assert.AreEqual(TerminationState.Filtered, sut.TerminatedState);
        }

        [Test]
        public void AddSelection()
        {
            var sut = new ComplEvent {TriggeredAt = DateTime.Now};
            sut.AddSelection(SomeProposal);
            Assert.AreEqual(1, sut.Selections.Count);
            var actual = sut.Selections[0];
            Assert.AreEqual(SomeProposal, actual.Proposal);
            Assert.True(actual.SelectedAfter.HasValue);
            Assert.Less(actual.SelectedAfter.Value.Milliseconds, 50);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ComplEvent();
            var b = new ComplEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new ComplEvent
            {
                Context2 = SomeContext,
                ProposalCollection = {new Proposal()},
                Prefix = "x",
                Selections = {new ProposalSelection()},
                TerminatedBy = IDEEvent.Trigger.Click,
                TerminatedState = TerminationState.Filtered
            };
            var b = new ComplEvent
            {
                Context2 = SomeContext,
                ProposalCollection = {new Proposal()},
                Prefix = "x",
                Selections = {new ProposalSelection()},
                TerminatedBy = IDEEvent.Trigger.Click,
                TerminatedState = TerminationState.Filtered
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentContext()
        {
            var a = new ComplEvent {Context2 = SomeContext};
            var b = new ComplEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProposalCollection()
        {
            var a = new ComplEvent {ProposalCollection = {new Proposal()}};
            var b = new ComplEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentPrefix()
        {
            var a = new ComplEvent {Prefix = "x"};
            var b = new ComplEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSelections()
        {
            var a = new ComplEvent {Selections = {new ProposalSelection()}};
            var b = new ComplEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTerminatedBy()
        {
            var a = new ComplEvent {TerminatedBy = IDEEvent.Trigger.Click};
            var b = new ComplEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTErminationState()
        {
            var a = new ComplEvent {TerminatedState = TerminationState.Filtered};
            var b = new ComplEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
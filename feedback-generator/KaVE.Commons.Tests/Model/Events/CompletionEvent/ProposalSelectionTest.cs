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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.CompletionEvent
{
    internal class ProposalSelectionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ProposalSelection();
            Assert.AreEqual(new Proposal(), sut.Proposal);
            Assert.AreEqual(ProposalSelection.DefaultIndex, sut.Index);
            Assert.False(sut.SelectedAfter.HasValue);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues_CustomConstructor()
        {
            var sut = new ProposalSelection(new Proposal {Relevance = 1});
            Assert.AreEqual(new Proposal {Relevance = 1}, sut.Proposal);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ProposalSelection
            {
                Proposal = new Proposal {Relevance = 1},
                SelectedAfter = TimeSpan.FromMinutes(2),
                Index = 3
            };
            Assert.AreEqual(new Proposal {Relevance = 1}, sut.Proposal);
            Assert.AreEqual(TimeSpan.FromMinutes(2), sut.SelectedAfter);
            Assert.AreEqual(3, sut.Index);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ProposalSelection();
            var b = new ProposalSelection();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_SettingValues()
        {
            var a = new ProposalSelection
            {
                Proposal = new Proposal {Relevance = 1},
                SelectedAfter = TimeSpan.FromMinutes(2),
                Index = 3
            };
            var b = new ProposalSelection
            {
                Proposal = new Proposal {Relevance = 1},
                SelectedAfter = TimeSpan.FromMinutes(2),
                Index = 3
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProposal()
        {
            var a = new ProposalSelection {Proposal = new Proposal {Relevance = 1}};
            var b = new ProposalSelection();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSelectedAfter()
        {
            var a = new ProposalSelection {SelectedAfter = TimeSpan.FromMinutes(2)};
            var b = new ProposalSelection();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSelectedAfterZero()
        {
            var a = new ProposalSelection {SelectedAfter = TimeSpan.FromMinutes(0)};
            var b = new ProposalSelection();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIndices()
        {
            var a = new ProposalSelection {Index = 1};
            var b = new ProposalSelection();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new ProposalSelection());
        }
    }
}
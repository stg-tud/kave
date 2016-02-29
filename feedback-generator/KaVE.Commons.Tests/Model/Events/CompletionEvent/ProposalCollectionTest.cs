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

using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.CompletionEvent
{
    internal class ProposalCollectionTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ProposalCollection();
            Assert.AreEqual(Lists.NewList<Proposal>(), sut.Proposals);
            Assert.AreEqual(0, sut.Count);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ProposalCollection
            {
                Proposals = {new Proposal()}
            };
            Assert.AreEqual(Lists.NewList(new Proposal()), sut.Proposals);
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public void SettingValues_CustomConstructor()
        {
            var sut = new ProposalCollection(Lists.NewList(new Proposal()));
            Assert.AreEqual(Lists.NewList(new Proposal()), sut.Proposals);
        }

        [Test]
        public void SettingValues_Initializer()
        {
            var sut = new ProposalCollection
            {
                new Proposal()
            };
            Assert.AreEqual(Lists.NewList(new Proposal()), sut.Proposals);
        }

        [Test]
        public void GetPosition()
        {
            var sut = new ProposalCollection
            {
                new Proposal {Relevance = 1},
                new Proposal {Relevance = 2}
            };
            Assert.AreEqual(1, sut.GetPosition(new Proposal {Relevance = 2}));
        }

        [Test]
        public void GetPosition_NotFound()
        {
            var sut = new ProposalCollection();
            Assert.AreEqual(-1, sut.GetPosition(new Proposal()));
        }

        [Test, Ignore]
        public void GetEnumerator()
        {
            // TODO how to get coverage for this case?
        }

        [Test]
        public void GetEnumerator_Generic()
        {
            var sut = new ProposalCollection
            {
                new Proposal {Relevance = 1},
                new Proposal {Relevance = 2}
            };
            var e = sut.GetEnumerator();
            Assert.Null(e.Current);
            Assert.True(e.MoveNext());
            Assert.AreEqual(new Proposal {Relevance = 1}, e.Current);
            Assert.True(e.MoveNext());
            Assert.AreEqual(new Proposal {Relevance = 2}, e.Current);
            Assert.False(e.MoveNext());
            Assert.Null(e.Current);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ProposalCollection();
            var b = new ProposalCollection();
            Assert.True(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_SettingValues()
        {
            var a = new ProposalCollection {new Proposal()};
            var b = new ProposalCollection {new Proposal()};
            Assert.True(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProposals()
        {
            var a = new ProposalCollection {new Proposal()};
            var b = new ProposalCollection();
            Assert.False(a.Equals(b));
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
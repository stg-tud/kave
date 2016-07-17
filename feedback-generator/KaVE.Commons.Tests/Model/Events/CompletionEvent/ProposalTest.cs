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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.CompletionEvent
{
    internal class ProposalTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new Proposal();
            Assert.Null(sut.Name);
            Assert.False(sut.Relevance.HasValue);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new Proposal
            {
                Name = Names.General("a"),
                Relevance = 3
            };
            Assert.AreEqual(Names.General("a"), sut.Name);
            Assert.AreEqual(3, sut.Relevance);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new Proposal();
            var b = new Proposal();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new Proposal
            {
                Name = Names.General("a"),
                Relevance = 3
            };
            var b = new Proposal
            {
                Name = Names.General("a"),
                Relevance = 3
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new Proposal
            {
                Name = Names.General("a")
            };
            var b = new Proposal();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentRelevance()
        {
            var a = new Proposal
            {
                Relevance = 3
            };
            var b = new Proposal();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentRelevanceWithZero()
        {
            var a = new Proposal {Relevance = 0};
            var b = new Proposal();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new Proposal());
        }
    }
}
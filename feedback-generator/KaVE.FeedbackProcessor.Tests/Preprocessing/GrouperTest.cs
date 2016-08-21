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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing
{
    internal class GrouperTest
    {
        #region setup and helper

        private IDictionary<string, IKaVESet<string>> _zipToIds;
        private const int MaxShuffleIterations = 10;

        [SetUp]
        public void Setup()
        {
            _zipToIds = new Dictionary<string, IKaVESet<string>>();
        }

        private void Given(string zip, params string[] ids)
        {
            _zipToIds[zip] = ToSet(ids);
        }

        private static IKaVESet<string> ToSet(params string[] ids)
        {
            return Sets.NewHashSetFrom(ids);
        }

        private void Expect(params IKaVESet<string>[] expecteds)
        {
            for (var i = 0; i < MaxShuffleIterations; i++)
            {
                var shuffledZipToIds = new Dictionary<string, IKaVESet<string>>();
                var shuffledZips = _zipToIds.Keys.OrderBy(k => Guid.NewGuid());
                foreach (var zip in shuffledZips)
                {
                    shuffledZipToIds[zip] = _zipToIds[zip];
                }
                ExpectSpecific(shuffledZipToIds, expecteds);
            }
        }

        private static void ExpectSpecific(IDictionary<string, IKaVESet<string>> zipToIds,
            params IKaVESet<string>[] expecteds)
        {
            var actuals = new Grouper().GroupRelatedZips(zipToIds);
            if (expecteds.Length != actuals.Count)
            {
                Assert.Fail("incorrect number of groups, expected {0}, but was {1}", expecteds.Length, actuals.Count);
            }
            foreach (var expected in expecteds)
            {
                if (!actuals.Contains(expected))
                {
                    Assert.Fail("expected to find {0} in {1}", expected, actuals);
                }
            }
        }

        #endregion

        [Test]
        public void NoOverlap()
        {
            Given("a", "a1");
            Given("b", "b1");

            Expect(ToSet("a"), ToSet("b"));
        }

        [Test]
        public void Overlap()
        {
            Given("a", "a1");
            Given("b", "a1");

            Expect(ToSet("a", "b"));
        }

        [Test]
        public void TransitiveOverlap()
        {
            Given("a", "0", "1");
            Given("b", "1", "2");
            Given("c", "2", "3");

            Expect(ToSet("a", "b", "c"));
        }

        [Test]
        public void TransitiveOverlap_reverse()
        {
            Given("c", "2", "3");
            Given("b", "1", "2");
            Given("a", "0", "1");

            Expect(ToSet("a", "b", "c"));
        }

        [Test]
        public void Overlap_Tree()
        {
            Given("root", "0");
            // depth = 1
            Given("l", "0", "0l", "1");
            Given("r", "0", "0r", "2");
            // depth = 2
            Given("ll", "0ll", "0l", "3");
            Given("lr", "0lr", "0l", "4");
            Given("rl", "0rl", "0r", "5");
            Given("rr", "0rr", "0r", "6");

            Given("other");

            Expect(ToSet("root", "l", "r", "ll", "lr", "rl", "rr"), ToSet("other"));
        }

        [Test]
        public void ShouldHandleZipsWithoutIds()
        {
            Given("a");
            Given("b");

            Expect(ToSet("a"), ToSet("b"));
        }

        [Test]
        public void OrderIsPreserved()
        {
            Given("a", "0", "1");
            Given("b", "1", "2");
            Given("c", "2", "3");

            var actuals = new Grouper().GroupRelatedZips(_zipToIds);
            Assert.AreEqual(1, actuals.Count);
            var actual = actuals.First();
            var expected = ToSet("a", "b", "c");
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
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
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing
{
    internal class GrouperTest
    {
        #region setup and helper

        private IDictionary<string, IKaVESet<string>> _zipToIds;
        private const int MaxShuffleIterations = 10;
        private IGrouperLogger _log;

        [SetUp]
        public void Setup()
        {
            _zipToIds = new Dictionary<string, IKaVESet<string>>();
            _log = Mock.Of<IGrouperLogger>();
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

        private void ExpectSpecific(IDictionary<string, IKaVESet<string>> zipToIds,
            params IKaVESet<string>[] expecteds)
        {
            var actuals = new Grouper(_log).GroupRelatedZips(zipToIds);
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

            var actuals = new Grouper(_log).GroupRelatedZips(_zipToIds);
            Assert.AreEqual(1, actuals.Count);
            var actual = actuals.First();
            var expected = ToSet("a", "b", "c");
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GrouperShouldNotChangeInput()
        {
            Setup();
            Given("a", "0", "1");
            Given("b", "1", "2");
            Given("c", "2", "3");
            new Grouper(_log).GroupRelatedZips(_zipToIds);
            var actual = _zipToIds;

            Setup();
            Given("a", "0", "1");
            Given("b", "1", "2");
            Given("c", "2", "3");
            var expected = _zipToIds;

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void LoggerTest()
        {
            var actualZips = new IDictionary<string, IKaVESet<string>>[1];
            var actualUsers = new IKaVESet<User>[1];

            Mock.Get(_log)
                .Setup(l => l.Zips(It.IsAny<IDictionary<string, IKaVESet<string>>>()))
                .Callback<IDictionary<string, IKaVESet<string>>>(m => { actualZips[0] = m; });
            Mock.Get(_log)
                .Setup(l => l.Users(It.IsAny<IKaVESet<User>>()))
                .Callback<IKaVESet<User>>(us => { actualUsers[0] = us; });

            Given("a", "0", "1");
            Given("b", "1", "2");
            Given("c", "3", "4");
            new Grouper(_log).GroupRelatedZips(_zipToIds);

            Mock.Get(_log).Verify(l => l.Init(), Times.Exactly(1));
            Mock.Get(_log).Verify(l => l.Zips(It.IsAny<IDictionary<string, IKaVESet<string>>>()), Times.Exactly(1));
            Mock.Get(_log).Verify(l => l.Users(It.IsAny<IKaVESet<User>>()), Times.Exactly(1));

            // zips
            var expectedZips = new Dictionary<string, IKaVESet<string>>
            {
                {"a", Sets.NewHashSet("0", "1")},
                {"b", Sets.NewHashSet("1", "2")},
                {"c", Sets.NewHashSet("3", "4")}
            };
            CollectionAssert.AreEqual(expectedZips, actualZips[0]);

            // users
            var expectedUsers = Sets.NewHashSet(
                new User {Files = {"a", "b"}, Identifiers = {"0", "1", "2"}},
                new User {Files = {"c"}, Identifiers = {"3", "4"}});
            CollectionAssert.AreEquivalent(expectedUsers, actualUsers[0]);
        }
    }
}
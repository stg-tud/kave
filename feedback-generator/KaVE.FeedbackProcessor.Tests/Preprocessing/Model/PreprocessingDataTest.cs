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

using System.Collections.Generic;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing.Model
{
    internal class PreprocessingDataTest
    {
        #region setup and helpers

        private IKaVESet<string> _zips;
        private PreprocessingData _sut;

        [SetUp]
        public void Setup()
        {
            _zips = Sets.NewHashSet<string>();
        }

        private void Init(params string[] zips)
        {
            _zips.AddAll(zips);
            _sut = new PreprocessingData(_zips);
        }

        private static IKaVEList<string> ToList(params string[] ids)
        {
            return Lists.NewListFrom(ids);
        }

        private static IKaVESet<string> ToSet(params string[] ids)
        {
            return Sets.NewHashSetFrom(ids);
        }

        #endregion

        [Test]
        public void ShouldIterateOverAllRegisteredZips()
        {
            Init("a", "b");

            var expecteds = ToSet("a", "b");
            var actuals = ToList(); // allow duplicates

            string zip;
            while (_sut.AcquireNextUnindexedZip(out zip))
            {
                actuals.Add(zip);
            }

            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test]
        public void ShouldStoreIdsByZip()
        {
            Init("a", "b");

            _sut.StoreIds("a", ToSet("a1"));
            _sut.StoreIds("b", ToSet("b1", "b2"));

            var actuals = _sut.GetIdsByZip();
            var expecteds = new Dictionary<string, IKaVESet<string>>
            {
                {"a", ToSet("a1")},
                {"b", ToSet("b1", "b2")}
            };
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectStoreIdsForUnknownZip()
        {
            Init();
            _sut.StoreIds("a", Sets.NewHashSet<string>());
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectRepeatedStoreIds()
        {
            Init("a");
            _sut.StoreIds("a", Sets.NewHashSet<string>());
            _sut.StoreIds("a", Sets.NewHashSet<string>());
        }

        [Test]
        public void ShouldStoreZipGroups()
        {
            Init("a", "b", "c");

            _sut.StoreZipGroups(Sets.NewHashSet(ToSet("a", "b"), ToSet("c")));

            var actuals = new KaVEList<IKaVESet<string>>(); // allow duplicates
            IKaVESet<string> zipGroup;
            while (_sut.AcquireNextUnmergedZipGroup(out zipGroup))
            {
                actuals.Add(zipGroup);
            }
            var expecteds = Sets.NewHashSet(ToSet("a", "b"), ToSet("c"));
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectEmptyZipGrouping()
        {
            _sut.StoreZipGroups(Sets.NewHashSet<IKaVESet<string>>());
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectEmptyZipGroups()
        {
            _sut.StoreZipGroups(Sets.NewHashSet(ToSet()));
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectZipGroupsWithUnknownElements()
        {
            _sut.StoreZipGroups(Sets.NewHashSet(ToSet("a")));
        }

        [Test]
        public void ShouldStoreMergedZip()
        {
            Init("a", "b");

            _sut.StoreZipGroups(Sets.NewHashSet(ToSet("b")));
            _sut.StoreMergedZip("a");

            var actuals = ToList(); // allow duplicates
            string zip;
            while (_sut.AcquireNextUncleansedZip(out zip))
            {
                actuals.Add(zip);
            }
            var expecteds = ToSet("a");
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectStoreUnknownZip()
        {
            Init();
            _sut.StoreMergedZip("a");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectRepeatedStoreMerged()
        {
            Init("a");
            _sut.StoreMergedZip("a");
            _sut.StoreMergedZip("a");
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectStoreMergedForElementsNotYetAcquired()
        {
            Init("a");
            _sut.StoreZipGroups(Sets.NewHashSet(ToSet("a")));
            _sut.StoreMergedZip("a");
        }
    }
}
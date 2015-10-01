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
using KaVE.Commons.Utils.Histograms;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Histograms
{
    internal class HistogramTest
    {
        [Test]
        public void EmptyHistogram()
        {
            var h = new Histogram(3);
            Assert.True(h.IsEmpty);
            Assert.AreEqual(0, h.NumValues);
            CollectionAssert.AreEquivalent(_(0, 0, 0), h.Values);
        }

        [Test]
        public void AddingValue()
        {
            var h = new Histogram(4);
            h.Add(2);
            Assert.False(h.IsEmpty);
            Assert.AreEqual(1, h.NumValues);
            CollectionAssert.AreEquivalent(_(0, 1, 0, 0), h.Values);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void AddingTooSmallValues()
        {
            var h = new Histogram(1);
            h.Add(0);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void AddingTooBigValues()
        {
            var h = new Histogram(1);
            h.Add(2);
        }

        [Test]
        public void PercentageValues_Empty()
        {
            var h = new Histogram(3);
            CollectionAssert.AreEquivalent(_(0d, 0d, 0d), h.ValuesRelative);
        }

        [Test]
        public void PercentageValues_Several()
        {
            var h = new Histogram(3);
            h.Add(1);
            h.Add(2);
            h.Add(2);
            AreEquivalent(_(0.333, 0.666, 0d), h.ValuesRelative);
        }

        private void AreEquivalent(IDictionary<int, double> a, IDictionary<int, double> b)
        {
            Assert.AreEqual(a.Keys, b.Keys);
            foreach (var k in a.Keys)
            {
                Assert.AreEqual(a[k], b[k], 0.002);
            }
        }

        private static IDictionary<int, int> _(params int[] vals)
        {
            var dict = new Dictionary<int, int>();
            var i = 1;
            foreach (var val in vals)
            {
                dict[i++] = val;
            }
            return dict;
        }

        private static IDictionary<int, double> _(params double[] vals)
        {
            var dict = new Dictionary<int, double>();
            var i = 1;
            foreach (var val in vals)
            {
                dict[i++] = val;
            }
            return dict;
        }
    }
}
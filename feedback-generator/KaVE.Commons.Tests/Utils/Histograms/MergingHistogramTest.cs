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

using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Histograms;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Histograms
{
    internal class MergingHistogramTest
    {
        private MergingHistogram _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new MergingHistogram(3);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void Error_DenominatorSmallerThanNumSlots()
        {
            _sut.AddRatio(1, 2);
        }

        [TestCase(1, 5, 1), TestCase(2, 5, 2), TestCase(3, 5, 2), TestCase(4, 5, 3), TestCase(5, 5, 3),
         TestCase(1, 6, 1), TestCase(2, 6, 1), TestCase(3, 6, 2), TestCase(4, 6, 2), TestCase(5, 6, 3),
         TestCase(6, 6, 3)]
        public void Example(int enumerator, int denominator, int expectedSlot)
        {
            _sut.AddRatio(enumerator, denominator);
            Assert.AreEqual(1, _sut.Values[expectedSlot]);
        }
    }
}
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

using KaVE.Commons.Utils.DateTimes;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.DateTimes
{
    internal class SimilarDateTimeComparerTest
    {
        [TestCase(5, 5)]
        [TestCase(3, 5)]
        [TestCase(0, 0)]
        [TestCase(-1, 5)]
        [TestCase(-5, 5)]
        public void ShouldBeEqualIf(int millisDifferenceToSecondDateTime, int equalityThreshold)
        {
            var firstDate = System.DateTime.Now;
            var secondDate = firstDate.AddMilliseconds(millisDifferenceToSecondDateTime);

            var comparer = new SimilarDateTimeComparer(equalityThreshold);
            Assert.AreEqual(0, comparer.Compare(firstDate, secondDate));
            Assert.IsTrue(comparer.Equal(firstDate, secondDate));
        }

        [TestCase(42, 5)]
        [TestCase(6, 5)]
        [TestCase(1, 0)]
        public void ShouldBeEarlierIf(int millisDifferenceToSecondDateTime, int equalityThreshold)
        {
            var firstDate = System.DateTime.Now;
            var secondDate = firstDate.AddMilliseconds(millisDifferenceToSecondDateTime);

            var comparer = new SimilarDateTimeComparer(equalityThreshold);
            Assert.AreEqual(-1, comparer.Compare(firstDate, secondDate));
            Assert.IsFalse(comparer.Equal(firstDate, secondDate));
        }

        [TestCase(-42, 5)]
        [TestCase(-6, 5)]
        [TestCase(-1, 0)]
        public void ShouldBeLaterIf(int millisDifferenceToSecondDateTime, int equalityThreshold)
        {
            var firstDate = System.DateTime.Now;
            var secondDate = firstDate.AddMilliseconds(millisDifferenceToSecondDateTime);

            var comparer = new SimilarDateTimeComparer(equalityThreshold);
            Assert.AreEqual(1, comparer.Compare(firstDate, secondDate));
            Assert.IsFalse(comparer.Equal(firstDate, secondDate));
        }
    }
}
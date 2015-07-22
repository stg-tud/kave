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

using System.Collections;
using System.Collections.Generic;
using KaVE.VS.Achievements.Util;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Util
{
    internal class FixedSizePriorityQueueTest
    {
        [Test]
        public void ShouldEnqueueNormally()
        {
            var uut = new FixedSizePriorityQueue<int>(1, new TestIntegerComparer());

            uut.Enqueue(0);

            Assert.AreEqual((int) 0, (int) uut.Dequeue());
        }

        [Test]
        public void ShouldDequeueWhenLimitIsReached()
        {
            var uut = new FixedSizePriorityQueue<int>(1, new TestIntegerComparer());

            uut.Enqueue(0);
            uut.Enqueue(1);

            Assert.AreEqual((int) 1, (int) uut.Dequeue());
        }

        [Test]
        public void ShouldSortAfterEnqueue()
        {
            var integerComparer = new TestIntegerComparer();

            var uut = new FixedSizePriorityQueue<int>(2, integerComparer);

            const int lowerValue = int.MinValue;
            const int higherValue = int.MaxValue;

            uut.Enqueue(higherValue);
            uut.Enqueue(lowerValue);

            Assert.AreEqual((int) lowerValue, (int) uut.Dequeue());
        }

        [Test]
        public void ShouldBeEmptyAfterClear()
        {
            var uut = new FixedSizePriorityQueue<int>(1, new TestIntegerComparer());

            uut.Enqueue(0);
            uut.Clear();

            Assert.IsEmpty((IEnumerable) uut.Items);
        }

        private class TestIntegerComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y);
            }
        }
    }
}
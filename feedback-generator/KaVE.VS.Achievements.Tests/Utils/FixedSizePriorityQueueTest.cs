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

using JetBrains;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.Achievements.Utils;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Utils
{
    internal class FixedSizePriorityQueueTest
    {
        private static Comparer<int> StandardIntComparer
        {
            get { return Comparer.Create<int>((x, y) => x.CompareTo(y)); }
        }

        [Test]
        public void ShouldEnqueueNormally()
        {
            var uut = new FixedSizePriorityQueue<int>(1, StandardIntComparer);

            uut.Enqueue(0);

            Assert.AreEqual(0, uut.Dequeue());
        }

        [Test]
        public void ShouldDequeueWhenLimitIsReached()
        {
            var uut = new FixedSizePriorityQueue<int>(1, StandardIntComparer);

            uut.Enqueue(0);
            uut.Enqueue(1);

            Assert.AreEqual(1, uut.Dequeue());
        }

        [Test]
        public void ShouldSortAfterEnqueue()
        {
            var uut = new FixedSizePriorityQueue<int>(2, StandardIntComparer);

            const int lowerValue = int.MinValue;
            const int higherValue = int.MaxValue;

            uut.Enqueue(higherValue);
            uut.Enqueue(lowerValue);

            Assert.AreEqual(lowerValue, uut.Dequeue());
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldThrowIfLimitIsSmallerThanZero()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new FixedSizePriorityQueue<int>(-1, StandardIntComparer);
        }

        [Test]
        public void ShouldBeEmptyAfterClear()
        {
            var uut = new FixedSizePriorityQueue<int>(1, StandardIntComparer);

            uut.Enqueue(0);
            uut.Clear();

            Assert.IsEmpty(uut.Items);
        }
    }
}
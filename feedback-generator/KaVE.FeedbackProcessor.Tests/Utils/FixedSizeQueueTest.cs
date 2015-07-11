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

using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Utils
{
    internal class FixedSizeQueueTest
    {
        [Test]
        public void ShouldEnqueueNormally()
        {
            var uut = new FixedSizeQueue<int>(1);

            uut.Enqueue(0);

            Assert.AreEqual(0, uut.Dequeue());
        }

        [Test]
        public void ShouldDequeueWhenLimitIsReached()
        {
            var uut = new FixedSizeQueue<int>(1);

            uut.Enqueue(0);
            uut.Enqueue(1);

            Assert.AreEqual(1, uut.Dequeue());
        }
    }
}
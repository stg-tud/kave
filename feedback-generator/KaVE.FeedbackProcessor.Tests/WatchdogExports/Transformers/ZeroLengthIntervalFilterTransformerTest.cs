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
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    internal class ZeroLengthIntervalFilterTransformerTest : TransformerTestBase<TestInterval>
    {
        [Test]
        public void PassesEventsToSubTransformer()
        {
            var testEvent = TestIDEEvent(0, 1);

            var subTransformer = Mock.Of<IEventToIntervalTransformer<TestInterval>>();
            var sut = new ZeroLengthIntervalFilterTransformer(subTransformer);

            sut.ProcessEvent(testEvent);
            Mock.Get(subTransformer).Verify(t => t.ProcessEvent(testEvent));
        }

        [Test]
        public void FiltersZeroLengthIntervals()
        {
            var goodInterval = new TestInterval {StartTime = DateTime.Now, Duration = TimeSpan.FromMinutes(1)};
            var badInterval = new TestInterval {StartTime = DateTime.Now, Duration = TimeSpan.Zero};

            var mockTransformer = Mock.Of<IEventToIntervalTransformer<Interval>>();
            Mock.Get(mockTransformer)
                .Setup(t => t.SignalEndOfEventStream())
                .Returns(new[] {goodInterval, badInterval});

            var sut = new ZeroLengthIntervalFilterTransformer(mockTransformer);

            CollectionAssert.AreEqual(new[] {goodInterval}, sut.SignalEndOfEventStream());
        }
    }
}
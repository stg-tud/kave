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

using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    internal class AggregateTransformerTest : TransformerTestBase<TestInterval>
    {
        [Test]
        public void PassesEventsToSubTransformers()
        {
            var testEvent = TestIDEEvent(0, 1);

            var subTransformer1 = Mock.Of<IEventToIntervalTransformer<TestInterval>>();
            var subTransformer2 = Mock.Of<IEventToIntervalTransformer<TestInterval>>();
            var sut = new AggregateTransformer(subTransformer1, subTransformer2);

            sut.ProcessEvent(testEvent);
            Mock.Get(subTransformer1).Verify(t => t.ProcessEvent(testEvent));
            Mock.Get(subTransformer2).Verify(t => t.ProcessEvent(testEvent));
        }

        [Test]
        public void ReturnsAllIntervalsFromSubTransformers()
        {
            var testInterval1 = ExpectedInterval(0, 1);
            var testInterval2 = ExpectedInterval(2, 3);

            var subTransformer1 = Mock.Of<IEventToIntervalTransformer<TestInterval>>();
            Mock.Get(subTransformer1).Setup(t => t.SignalEndOfEventStream()).Returns(new[] {testInterval1});
            var subTransformer2 = Mock.Of<IEventToIntervalTransformer<TestInterval>>();
            Mock.Get(subTransformer2).Setup(t => t.SignalEndOfEventStream()).Returns(new[] {testInterval2});

            var sut = new AggregateTransformer(subTransformer1, subTransformer2);

            CollectionAssert.AreEquivalent(new[] {testInterval1, testInterval2}, sut.SignalEndOfEventStream());
        }
    }
}
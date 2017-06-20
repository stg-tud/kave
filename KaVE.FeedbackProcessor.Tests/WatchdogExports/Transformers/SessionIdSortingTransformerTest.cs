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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    internal class SessionIdSortingTransformerTest : TransformerTestBase<TestInterval>
    {
        [Test]
        public void CreatesNewSubTransformerForNewId()
        {
            var count = 0;

            var sut = new SessionIdSortingTransformer<TestInterval>(
                () =>
                {
                    count++;
                    return Mock.Of<IEventToIntervalTransformer<TestInterval>>();
                });

            sut.ProcessEvent(TestIDEEvent(0, 0, "a"));
            sut.ProcessEvent(TestIDEEvent(0, 0, "a"));
            sut.ProcessEvent(TestIDEEvent(0, 0, "b"));

            Assert.AreEqual(2, count);
        }

        [Test]
        public void PassesEventsToSubTransformer()
        {
            var events = new[]
            {
                TestIDEEvent(0, 0, "a"),
                TestIDEEvent(1, 1, "a"),
                TestIDEEvent(0, 0, "b")
            };

            var processingInstances = new Dictionary<string, IEventToIntervalTransformer<TestInterval>>();
            var processedEvents = new List<IDEEvent>();

            var sut = new SessionIdSortingTransformer<TestInterval>(
                () =>
                {
                    var transformer = Mock.Of<IEventToIntervalTransformer<TestInterval>>();
                    Mock.Get(transformer).Setup(t => t.ProcessEvent(It.IsAny<IDEEvent>())).Callback<IDEEvent>(
                            e =>
                            {
                                processedEvents.Add(e);

                                if (processingInstances.ContainsKey(e.IDESessionUUID))
                                {
                                    if (processingInstances[e.IDESessionUUID] != transformer)
                                    {
                                        Assert.Fail();
                                    }
                                }
                                else
                                {
                                    processingInstances.Add(e.IDESessionUUID, transformer);
                                }
                            });
                    return transformer;
                });

            foreach (var e in events)
            {
                sut.ProcessEvent(e);
            }

            CollectionAssert.AreEquivalent(events, processedEvents);
        }

        [Test]
        public void ReturnsIntervalsFromSubTransformers()
        {
            var sut = new SessionIdSortingTransformer<TestInterval>(
                () =>
                {
                    var transformer = Mock.Of<IEventToIntervalTransformer<TestInterval>>();
                    Mock.Get(transformer).Setup(t => t.SignalEndOfEventStream()).Returns(new[] {ExpectedInterval(0, 0)});
                    return transformer;
                });

            sut.ProcessEvent(TestIDEEvent(0, 0, "a"));
            sut.ProcessEvent(TestIDEEvent(0, 0, "a"));
            sut.ProcessEvent(TestIDEEvent(0, 0, "b"));

            Assert.AreEqual(2, sut.SignalEndOfEventStream().Count());
        }
    }
}
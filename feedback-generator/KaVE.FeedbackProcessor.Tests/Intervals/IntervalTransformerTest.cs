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
using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Intervals;
using KaVE.FeedbackProcessor.Intervals.Model;
using KaVE.FeedbackProcessor.Intervals.Transformers;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Intervals
{
    internal class IntervalTransformerTest
    {
        [Test]
        public void DoesntProcessEventsWithInsufficientTimeData()
        {
            var goodEvent = new TestIDEEvent {TriggeredAt = DateTime.Now, TerminatedAt = DateTime.Now};
            var badEvent = new TestIDEEvent {TriggeredAt = DateTime.Now.AddMinutes(-1)};

            var processedEvents = new List<IDEEvent>();

            var mockTransformer = Mock.Of<IEventToIntervalTransformer<Interval>>();
            Mock.Get(mockTransformer)
                .Setup(t => t.ProcessEvent(It.IsAny<IDEEvent>()))
                .Callback<IDEEvent>(processedEvents.Add);

            new IntervalTransformer(new NullLogger()).TransformWithCustomTransformer(new[] {goodEvent, badEvent}, mockTransformer);

            CollectionAssert.AreEqual(new[] {goodEvent}, processedEvents);
        }

        [Test, ExpectedException(typeof (InvalidDataException))]
        public void FailsIfEventsAreNotOrdered()
        {
            var unorderedEvents = new[]
            {
                new TestIDEEvent {TriggeredAt = DateTime.Now, TerminatedAt = DateTime.Now},
                new TestIDEEvent {TriggeredAt = DateTime.Now.AddMinutes(-1), TerminatedAt = DateTime.Now}
            };

            var mockTransformer = Mock.Of<IEventToIntervalTransformer<Interval>>();

            new IntervalTransformer(new NullLogger()).TransformWithCustomTransformer(unorderedEvents, mockTransformer);
        }
    }
}
﻿/*
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
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.WatchdogExports;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using Moq;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports
{
    internal class IntervalTransformerTest
    {
        private IEventFixer _fixer;

        [SetUp]
        public void Setup()
        {
            _fixer = Mock.Of<IEventFixer>();
            Mock.Get(_fixer)
                .Setup(f => f.FixAndFilter(It.IsAny<IEnumerable<IDEEvent>>()))
                .Returns<IEnumerable<IDEEvent>>(es => es);
        }

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


            new IntervalTransformer(new NullLogger(), _fixer).TransformWithCustomTransformer(
                new[] {goodEvent, badEvent},
                mockTransformer);

            Mock.Get(_fixer).Verify(f => f.FixAndFilter(It.IsAny<IEnumerable<IDEEvent>>()));

            CollectionAssert.AreEqual(new[] {goodEvent}, processedEvents);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void FailsIfEventsAreNotOrdered()
        {
            var unorderedEvents = new[]
            {
                new TestIDEEvent {TriggeredAt = DateTime.Now, TerminatedAt = DateTime.Now},
                new TestIDEEvent {TriggeredAt = DateTime.Now.AddMinutes(-1), TerminatedAt = DateTime.Now}
            };

            var mockTransformer = Mock.Of<IEventToIntervalTransformer<Interval>>();

            new IntervalTransformer(new NullLogger(), _fixer).TransformWithCustomTransformer(
                unorderedEvents,
                mockTransformer);
        }
    }
}
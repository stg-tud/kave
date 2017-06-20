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
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    internal class IsolatedEventBlockFilterTest
    {
        private IsolatedEventBlockFilter _uut;

        [SetUp]
        public void Setup()
        {
            var longBreak = TimeSpan.FromMinutes(30);
            var maximumBlockSpan = TimeSpan.FromSeconds(1);

            _uut = new IsolatedEventBlockFilter(longBreak, maximumBlockSpan);
        }

        [Test]
        public void ShouldNotDropEventsWithoutPrecedingLongBreak()
        {
            var someEvent = TestEventFactory.SomeEvent(DateTimeFactory.SomeWorkingHoursDateTime());

            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(someEvent), _uut.Map(someEvent));
        }

        [Test]
        public void ShouldDropEventAfterLongBreak()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var eventBeforeBreak = TestEventFactory.SomeEvent(someDateTime);
            var eventAfterBreak = TestEventFactory.SomeEvent(someDateTime + _uut.LongBreak);

            _uut.Map(eventBeforeBreak);
            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(), _uut.Map(eventAfterBreak));
        }

        [Test]
        public void ShouldDropMultipleEventsAfterLongBreak()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var eventBeforeBreak = TestEventFactory.SomeEvent(someDateTime);
            var firstEventAfterBreak = TestEventFactory.SomeEvent(someDateTime + _uut.LongBreak);
            var secondEventAfterBreak = TestEventFactory.SomeEvent(someDateTime + _uut.LongBreak);

            _uut.Map(eventBeforeBreak);
            _uut.Map(firstEventAfterBreak);
            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(), _uut.Map(secondEventAfterBreak));
        }

        [Test]
        public void ShouldInsertDroppedEventsIfNoSecondLongBreakOccurs()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var eventBeforeBreak = TestEventFactory.SomeEvent(someDateTime);
            var droppedEvent = TestEventFactory.SomeEvent(someDateTime + _uut.LongBreak);
            var eventAfterBlockSpanWithoutLongBreak =
                TestEventFactory.SomeEvent(
                    someDateTime + _uut.LongBreak + _uut.MaximumBlockSpan + TimeSpan.FromTicks(1));

            _uut.Map(eventBeforeBreak);
            _uut.Map(droppedEvent);
            var actualSet = _uut.Map(eventAfterBlockSpanWithoutLongBreak);

            var expectedSet = Sets.NewHashSet<IDEEvent>(droppedEvent, eventAfterBlockSpanWithoutLongBreak);
            CollectionAssert.AreEquivalent(expectedSet, actualSet);
        }

        [Test]
        public void ShouldNotInsertDroppedEventsIfASecondLongBreakOccurs()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var eventBeforeBreak = TestEventFactory.SomeEvent(someDateTime);
            var droppedEvent = TestEventFactory.SomeEvent(someDateTime + _uut.LongBreak);
            var eventAfterSecondLongBreak = TestEventFactory.SomeEvent(
                someDateTime + _uut.LongBreak + _uut.LongBreak);

            _uut.Map(eventBeforeBreak);
            _uut.Map(droppedEvent);
            CollectionAssert.DoesNotContain(_uut.Map(eventAfterSecondLongBreak), droppedEvent);
        }

        [Test]
        public void ShouldAddDroppedEventBlocksToStatistic()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();

            var eventBeforeBreak = TestEventFactory.SomeEvent(someDateTime);
            var droppedEvent = TestEventFactory.SomeEvent(someDateTime + _uut.LongBreak);
            var eventAfterSecondLongBreak = TestEventFactory.SomeEvent(
                someDateTime + _uut.LongBreak + _uut.LongBreak);

            _uut.Map(eventBeforeBreak);
            _uut.Map(droppedEvent);
            _uut.Map(eventAfterSecondLongBreak);

            var expectedFilteredIsolatedBlocks = Lists.NewList(Lists.NewList<IDEEvent>(droppedEvent));
            CollectionAssert.AreEquivalent(_uut.FilteredIsolatedBlocks, expectedFilteredIsolatedBlocks);
        }
    }
}
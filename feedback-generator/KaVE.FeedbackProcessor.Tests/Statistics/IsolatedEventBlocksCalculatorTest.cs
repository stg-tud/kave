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
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    internal class IsolatedEventBlocksCalculatorTest
    {
        private IsolatedEventBlocksCalculator _uut;

        [SetUp]
        public void Setup()
        {
            var longBreak = TimeSpan.FromMinutes(30);
            var maximumBlockSpan = TimeSpan.FromSeconds(1);

            _uut = new IsolatedEventBlocksCalculator(longBreak, maximumBlockSpan);
        }

        [Test]
        public void ShouldAddDroppedEventBlocksToStatistic()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();

            var eventBeforeBreak = TestEventFactory.SomeEvent(someDateTime);
            var droppedEvent = TestEventFactory.SomeEvent(someDateTime + _uut.LongBreak);
            var eventAfterSecondLongBreak = TestEventFactory.SomeEvent(
                someDateTime + _uut.LongBreak + _uut.LongBreak);

            _uut.OnEvent(eventBeforeBreak);
            _uut.OnEvent(droppedEvent);
            _uut.OnEvent(eventAfterSecondLongBreak);

            var expectedFilteredIsolatedBlocks = Lists.NewList(Lists.NewList<IDEEvent>(droppedEvent));
            CollectionAssert.AreEquivalent(_uut.LoggedIsolatedBlocks, expectedFilteredIsolatedBlocks);
        }
    }
}
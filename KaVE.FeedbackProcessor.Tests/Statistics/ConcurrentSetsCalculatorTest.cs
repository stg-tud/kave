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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    internal class ConcurrentSetsCalculatorTest
    {
        private ConcurrentSetsCalculator _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new ConcurrentSetsCalculator();
        }

        [Test]
        public void CountsEqualSetsCorrectly()
        {
            var commandEvent = new CommandEvent {CommandId = "Test"};
            var documentEvent = new DocumentEvent {Action = DocumentAction.Saved};
            var concurrentEvent = new ConcurrentEvent
            {
                ConcurrentEventList = new List<IDEEvent>
                {
                    commandEvent,
                    documentEvent
                }
            };

            _uut.OnEvent(concurrentEvent);
            _uut.OnEvent(concurrentEvent);

            Assert.AreEqual(
                2,
                _uut.Statistic[
                    Sets.NewHashSet(
                        EventMappingUtils.GetAbstractStringOf(documentEvent),
                        EventMappingUtils.GetAbstractStringOf(commandEvent))]);
        }

        [Test]
        public void ShouldIgnoreAllOtherEvents()
        {
            _uut.OnEvent(TestEventFactory.SomeEvent());

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void ShouldNotAddEmptySetToStatistic()
        {
            var concurrentEvent = new ConcurrentEvent();

            _uut.OnEvent(concurrentEvent);

            CollectionAssert.IsEmpty(_uut.Statistic);
        }
    }
}
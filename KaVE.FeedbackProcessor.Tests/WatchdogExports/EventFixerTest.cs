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
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.WatchdogExports;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports
{
    internal class EventFixerTest
    {
        private EventFixer _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new EventFixer();
        }

        [Test]
        public void FiltersEventsWithoutTriggeredAt()
        {
            var actuals = _sut.FixAndFilter(new[] {new CommandEvent()});
            var expecteds = new IDEEvent[0];
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void ReturnsUnaffectedEventsUnchanged()
        {
            var actuals = _sut.FixAndFilter(new[] {new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(2)}});
            var expecteds = new[] {new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(2)}};
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void SetsTerminatedAtIfNotSet()
        {
            var actuals = _sut.FixAndFilter(new[] {new CommandEvent {TriggeredAt = Date(1)}});
            var expecteds = new[] {new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(1)}};
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void ResetsTerminatedAtForEditEvents()
        {
            var actuals = _sut.FixAndFilter(new[] {new EditEvent {TriggeredAt = Date(1), TerminatedAt = Date(123)}});
            var expecteds = new[] {new EditEvent {TriggeredAt = Date(1), TerminatedAt = Date(1)}};
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void AddsTimingInformationToTestCaseResults()
        {
            var actuals =
                _sut.FixAndFilter(
                    new[]
                    {
                        new TestRunEvent
                        {
                            TriggeredAt = Date(1),
                            Duration = Dur(13),
                            Tests =
                            {
                                new TestCaseResult {Duration = Dur(1), Parameters = "p1"},
                                new TestCaseResult {Duration = Dur(1), Parameters = "p2"}
                            }
                        }
                    });
            var expecteds = new[]
            {
                new TestRunEvent
                {
                    TriggeredAt = Date(1),
                    Duration = Dur(13),
                    Tests =
                    {
                        new TestCaseResult {StartTime = Date(1), Duration = Dur(1), Parameters = "p1"},
                        new TestCaseResult {StartTime = Date(2), Duration = Dur(1), Parameters = "p2"}
                    }
                }
            };
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void FiltersDuplicateCommandWithTheSameIdThatHappenAtTheSameTime_1()
        {
            var actuals =
                _sut.FixAndFilter(
                    new[]
                    {
                        new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(2), CommandId = "a:b:id1"},
                        new CommandEvent {TriggeredAt = Date(1), CommandId = "c:d:id1"}
                    });
            var expecteds = new[]
            {
                new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(2), CommandId = "a:b:id1"}
            };
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void FiltersDuplicateCommandWithTheSameIdThatHappenAtTheSameTime_2()
        {
            var actuals =
                _sut.FixAndFilter(
                    new IDEEvent[]
                    {
                        new CommandEvent {TriggeredAt = Date(1), CommandId = "c:d:id1"},
                        new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(2), CommandId = "a:b:id1"}
                    });
            var expecteds = new[]
            {
                new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(2), CommandId = "a:b:id1"}
            };
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void DoesNotFiltersDuplicateCommandWithTheSameIdIfTimeChagnes()
        {
            var actuals =
                _sut.FixAndFilter(
                    new[]
                    {
                        new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(3), CommandId = "a:b:id1"},
                        new CommandEvent {TriggeredAt = Date(2), TerminatedAt = Date(3), CommandId = "a:b:id1"}
                    });
            var expecteds = new[]
            {
                new CommandEvent {TriggeredAt = Date(1), TerminatedAt = Date(3), CommandId = "a:b:id1"},
                new CommandEvent {TriggeredAt = Date(2), TerminatedAt = Date(3), CommandId = "a:b:id1"}
            };
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        private DateTime Date(int i)
        {
            return DateTime.MinValue.AddSeconds(i);
        }

        private TimeSpan Dur(int i)
        {
            return DateTime.MinValue.AddSeconds(i) - DateTime.MinValue;
        }
    }
}
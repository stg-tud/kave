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
using KaVE.FeedbackProcessor.Intervals.Model;
using KaVE.FeedbackProcessor.Intervals.Transformers;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Intervals.Transformers
{
    internal class TestRunTransformerTest : TransformerTestBase<TestRunInterval>
    {
        [Test]
        public void CreatesInterval()
        {
            var commandEvent = new CommandEvent {CommandId = "UnitTest_RunContext", TriggeredAt = TestTime(0), TerminatedAt = TestTime(10)};

            var sut = new TestRunIntervalTransformer(_context);
            sut.ProcessEvent(commandEvent);

            var actual = sut.SignalEndOfEventStream();
            var expectedInterval = ExpectedInterval(0, 1);
            expectedInterval.Duration = TimeSpan.FromSeconds(1);
            var expected = new[] {expectedInterval};

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
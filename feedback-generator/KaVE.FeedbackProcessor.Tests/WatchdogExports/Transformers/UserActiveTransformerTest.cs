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
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    internal class UserActiveTransformerTest : TransformerTestBase<UserActiveInterval>
    {
        [Test]
        public void CreatesIntervalWithIdenticalTimeDataForSingleEvent()
        {
            var sut = new UserActiveTransformer(_context, TimeSpan.FromMinutes(16));

            sut.ProcessEvent(TestIDEEvent(0, 5, "a"));

            CollectionAssert.AreEqual(new[] {ExpectedInterval(0, 5, "a")}, sut.SignalEndOfEventStream());
        }

        [Test]
        public void CreatesMergedIntervalsForCloseEvents()
        {
            var sut = new UserActiveTransformer(_context, TimeSpan.FromMinutes(16));

            sut.ProcessEvent(TestIDEEvent(0, 5, "a"));
            sut.ProcessEvent(TestIDEEvent(3, 6, "a"));
            sut.ProcessEvent(TestIDEEvent(9, 10, "a"));
            sut.ProcessEvent(TestIDEEvent(26, 30, "a"));

            CollectionAssert.AreEqual(new[] {ExpectedInterval(0, 30, "a")}, sut.SignalEndOfEventStream());
        }

        [Test]
        public void CreatesSeperateIntervalsForEventsThatAreFurtherApart()
        {
            var sut = new UserActiveTransformer(_context, TimeSpan.FromMinutes(16));

            sut.ProcessEvent(TestIDEEvent(0, 10, "a"));
            sut.ProcessEvent(TestIDEEvent(27, 30, "a"));

            CollectionAssert.AreEqual(
                new[] {ExpectedInterval(0, 10, "a"), ExpectedInterval(27, 30, "a")},
                sut.SignalEndOfEventStream());
        }
    }
}
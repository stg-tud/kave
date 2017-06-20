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

using System.Linq;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    internal class VisualStudioOpenedTransformerTest : TransformerTestBase<VisualStudioOpenedInterval>
    {
        [Test]
        public void CreatesIntervalForEachSessionId()
        {
            var sut = new VisualStudioOpenedTransformer(_context);

            sut.ProcessEvent(TestIDEEvent(-3, -2, "a"));
            sut.ProcessEvent(TestIDEEvent(-1, 0, "b"));

            var intervals = sut.SignalEndOfEventStream().ToList();

            CollectionAssert.Contains(intervals, ExpectedInterval(-3, -2, "a"));
            CollectionAssert.Contains(intervals, ExpectedInterval(-1, 0, "b"));
        }

        [Test]
        public void IntervalsAreExpandedCorrectly()
        {
            var sut = new VisualStudioOpenedTransformer(_context);

            sut.ProcessEvent(TestIDEEvent(-3, -2, "a"));
            sut.ProcessEvent(TestIDEEvent(-1, 0, "a"));

            var intervals = sut.SignalEndOfEventStream().ToList();

            CollectionAssert.Contains(intervals, ExpectedInterval(-3, 0, "a"));
        }
    }
}
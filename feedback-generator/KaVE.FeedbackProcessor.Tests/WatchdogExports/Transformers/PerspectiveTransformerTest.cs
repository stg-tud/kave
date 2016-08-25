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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    internal class PerspectiveTransformerTest : TransformerTestBase<PerspectiveInterval>
    {
        private PerspectiveInterval ExpectedInterval(int startOffsetInMinutes,
            int endOffsetInMinutes,
            PerspectiveType type)
        {
            var interval = base.ExpectedInterval(startOffsetInMinutes, endOffsetInMinutes);
            interval.Perspective = type;
            return interval;
        }

        private DebuggerEvent TestDebuggerEvent(int startOffsetInMinutes, int endOffsetInMinutes, bool isStartOfSession)
        {
            return new DebuggerEvent
            {
                TriggeredAt = TestTime(startOffsetInMinutes),
                TerminatedAt = TestTime(endOffsetInMinutes),
                Mode = isStartOfSession ? DebuggerMode.Run : DebuggerMode.Design
            };
        }

        [Test]
        public void CreatesDebugPerspectiveInterval()
        {
            var sut = new PerspectiveTransformer(_context);

            sut.ProcessEvent(TestDebuggerEvent(0, 0, true));
            sut.ProcessEvent(TestIDEEvent(1, 2));
            sut.ProcessEvent(TestIDEEvent(3, 4));
            sut.ProcessEvent(TestDebuggerEvent(5, 5, false));

            var expected = ExpectedInterval(0, 5, PerspectiveType.Debug);

            CollectionAssert.AreEqual(new[] {expected}, sut.SignalEndOfEventStream());
        }

        [Test]
        public void CreatesMultipleDebugPerspectiveIntervals()
        {
            var sut = new PerspectiveTransformer(_context);

            sut.ProcessEvent(TestDebuggerEvent(0, 0, true));
            sut.ProcessEvent(TestDebuggerEvent(1, 1, false));
            sut.ProcessEvent(TestDebuggerEvent(2, 2, true));
            sut.ProcessEvent(TestDebuggerEvent(3, 3, false));

            var expected = new[]
            {
                ExpectedInterval(0, 1, PerspectiveType.Debug),
                ExpectedInterval(2, 3, PerspectiveType.Debug)
            };

            CollectionAssert.AreEqual(expected, sut.SignalEndOfEventStream());
        }

        [Test]
        public void CreatesProductionPerspectiveInterval()
        {
            var sut = new PerspectiveTransformer(_context);

            sut.ProcessEvent(TestIDEEvent(0, 1));
            sut.ProcessEvent(TestIDEEvent(2, 3));

            var expected = ExpectedInterval(0, 3, PerspectiveType.Production);

            CollectionAssert.AreEqual(new[] {expected}, sut.SignalEndOfEventStream());
        }

        [Test]
        public void CreatesInterleavedPerspectiveIntervals()
        {
            var sut = new PerspectiveTransformer(_context);

            sut.ProcessEvent(TestDebuggerEvent(0, 0, true));
            sut.ProcessEvent(TestDebuggerEvent(1, 1, false));
            sut.ProcessEvent(TestIDEEvent(2, 3));
            sut.ProcessEvent(TestDebuggerEvent(4, 5, true));
            sut.ProcessEvent(TestDebuggerEvent(4, 5, false));
            sut.ProcessEvent(TestIDEEvent(6, 7));
            sut.ProcessEvent(TestIDEEvent(7, 8));

            var expected = new[]
            {
                ExpectedInterval(0, 1, PerspectiveType.Debug),
                ExpectedInterval(2, 3, PerspectiveType.Production),
                ExpectedInterval(4, 5, PerspectiveType.Debug),
                ExpectedInterval(6, 8, PerspectiveType.Production)
            };

            CollectionAssert.AreEqual(expected, sut.SignalEndOfEventStream());
        }

        [Test]
        public void IntervalsDontOverlap()
        {
            var sut = new PerspectiveTransformer(_context);

            sut.ProcessEvent(TestDebuggerEvent(0, 2, true));
            sut.ProcessEvent(TestDebuggerEvent(4, 6, false));
            sut.ProcessEvent(TestIDEEvent(5, 7));

            var expected = new[]
            {
                ExpectedInterval(0, 6, PerspectiveType.Debug),
                ExpectedInterval(6, 7, PerspectiveType.Production)
            };

            CollectionAssert.AreEqual(expected, sut.SignalEndOfEventStream());
        }
    }
}
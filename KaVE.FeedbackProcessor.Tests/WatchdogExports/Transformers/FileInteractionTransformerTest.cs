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
using KaVE.Commons.Model.Naming;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    [Ignore("ignore these tests -for now- until the work on the interval export continues")]
    internal class FileInteractionTransformerTest : TransformerTestBase<FileInteractionInterval>
    {
        private BuildEvent TestReadingEvent(int startOffset, int endOffset, string filename)
        {
            return new BuildEvent
            {
                TriggeredAt = TestTime(startOffset),
                TerminatedAt = TestTime(endOffset),
                ActiveDocument = Names.Document("CSharp " + filename)
            };
        }

        private EditEvent TestTypingEvent(int startOffset, int endOffset, string filename)
        {
            return new EditEvent
            {
                TriggeredAt = TestTime(startOffset),
                TerminatedAt = TestTime(endOffset),
                ActiveDocument = Names.Document("CSharp " + filename)
            };
        }

        private DebuggerEvent TestDebuggerEvent(int startOffsetInMinutes,
            int endOffsetInMinutes,
            string filename,
            bool isStartOfSession)
        {
            return new DebuggerEvent
            {
                TriggeredAt = TestTime(startOffsetInMinutes),
                TerminatedAt = TestTime(endOffsetInMinutes),
                ActiveDocument = Names.Document("CSharp " + filename),
                Mode = isStartOfSession ? DebuggerMode.Run : DebuggerMode.Design
            };
        }

        private FileInteractionInterval ExpectedInterval(int startOffset, int endOffset, string filename, bool isTyping)
        {
            var interval = base.ExpectedInterval(startOffset, endOffset);
            interval.FileName = filename;
            interval.FileType = DocumentType.Production;
            interval.Type = isTyping ? FileInteractionType.Typing : FileInteractionType.Reading;
            return interval;
        }

        [Test]
        public void CreatesNewTypingIntervalWhenFileChanges()
        {
            var sut = new FileInteractionTransformer(_context);

            sut.ProcessEvent(TestTypingEvent(0, 1, "File1.cs"));
            sut.ProcessEvent(TestTypingEvent(1, 2, "File2.cs"));

            var expected = new[] {ExpectedInterval(0, 1, "File1.cs", true), ExpectedInterval(1, 2, "File2.cs", true)};

            Assert.AreEqual(expected, sut.SignalEndOfEventStream());
        }

        [Test]
        public void CreatesNewIntervalWhenTypeChanges()
        {
            var sut = new FileInteractionTransformer(_context);

            sut.ProcessEvent(TestReadingEvent(0, 1, "File1.cs"));
            sut.ProcessEvent(TestTypingEvent(1, 2, "File1.cs"));

            var expected = new[] {ExpectedInterval(0, 1, "File1.cs", false), ExpectedInterval(1, 2, "File1.cs", true)};

            var actual = sut.SignalEndOfEventStream();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreatesNewIntervalWhenBothCriteriaChange()
        {
            var sut = new FileInteractionTransformer(_context);

            sut.ProcessEvent(TestReadingEvent(0, 1, "File1.cs"));
            sut.ProcessEvent(TestTypingEvent(1, 2, "File2.cs"));

            var expected = new[] {ExpectedInterval(0, 1, "File1.cs", false), ExpectedInterval(1, 2, "File2.cs", true)};

            var actual = sut.SignalEndOfEventStream();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ResultingIntervalsDontOverlap()
        {
            var sut = new FileInteractionTransformer(_context);

            sut.ProcessEvent(TestReadingEvent(0, 2, "File1.cs"));
            sut.ProcessEvent(TestTypingEvent(1, 3, "File1.cs"));

            var expected = new[] {ExpectedInterval(0, 2, "File1.cs", false), ExpectedInterval(2, 3, "File1.cs", true)};

            Assert.AreEqual(expected, sut.SignalEndOfEventStream());
        }

        [Test]
        public void ResultingIntervalsDontOverlap2()
        {
            var sut = new FileInteractionTransformer(_context);

            sut.ProcessEvent(TestReadingEvent(0, 2, "File1.cs"));
            sut.ProcessEvent(TestTypingEvent(1, 3, "File2.cs"));

            var expected = new[] {ExpectedInterval(0, 2, "File1.cs", false), ExpectedInterval(2, 3, "File2.cs", true)};

            Assert.AreEqual(expected, sut.SignalEndOfEventStream());
        }

        [Test]
        public void IgnoresEditEventsWhileDebugging()
        {
            var sut = new FileInteractionTransformer(_context);

            sut.ProcessEvent(TestDebuggerEvent(0, 1, "File1.cs", true));
            sut.ProcessEvent(TestTypingEvent(1, 2, "File1.cs"));
            sut.ProcessEvent(TestDebuggerEvent(2, 3, "File1.cs", false));
            sut.ProcessEvent(TestTypingEvent(3, 4, "File1.cs"));

            var expected = new[] {ExpectedInterval(0, 3, "File1.cs", false), ExpectedInterval(3, 4, "File1.cs", true)};

            Assert.AreEqual(expected, sut.SignalEndOfEventStream());
        }
    }
}
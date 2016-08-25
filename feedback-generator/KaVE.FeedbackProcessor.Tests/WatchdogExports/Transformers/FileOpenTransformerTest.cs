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
    internal class FileOpenTransformerTest : TransformerTestBase<FileOpenInterval>
    {
        private DocumentEvent TestDocumentEvent(int startOffset,
            int endOffset,
            string filename,
            bool isFileOpen,
            string sessionId = null)
        {
            return new DocumentEvent
            {
                TriggeredAt = TestTime(startOffset),
                TerminatedAt = TestTime(endOffset),
                Action = isFileOpen ? DocumentAction.Opened : DocumentAction.Closing,
                Document = Names.Document("CSharp " + filename),
                IDESessionUUID = sessionId
            };
        }

        private FileOpenInterval ExpectedInterval(int startOffset,
            int endOffset,
            string filename,
            string sessionId = null)
        {
            var interval = base.ExpectedInterval(startOffset, endOffset, sessionId);
            interval.FileName = filename;
            return interval;
        }

        [Test]
        public void SingleInterval()
        {
            var sut = new FileOpenTransformer(_context);

            sut.ProcessEvent(TestDocumentEvent(0, 0, "File1.cs", true));
            sut.ProcessEvent(TestDocumentEvent(1, 1, "File1.cs", false));

            var expected = ExpectedInterval(0, 1, "File1.cs");
            CollectionAssert.AreEquivalent(new[] {expected}, sut.SignalEndOfEventStream());
        }

        [Test]
        public void MultipleIntervalsWithOverlaps()
        {
            var sut = new FileOpenTransformer(_context);

            sut.ProcessEvent(TestDocumentEvent(0, 0, "File1.cs", true));
            sut.ProcessEvent(TestDocumentEvent(4, 4, "File1.cs", false));

            sut.ProcessEvent(TestDocumentEvent(2, 2, "File2.cs", true));
            sut.ProcessEvent(TestDocumentEvent(6, 6, "File2.cs", false));

            sut.ProcessEvent(TestDocumentEvent(8, 8, "File1.cs", true));
            sut.ProcessEvent(TestDocumentEvent(10, 10, "File1.cs", false));

            var expected = new[]
            {
                ExpectedInterval(0, 4, "File1.cs"),
                ExpectedInterval(2, 6, "File2.cs"),
                ExpectedInterval(8, 10, "File1.cs")
            };

            CollectionAssert.AreEquivalent(expected, sut.SignalEndOfEventStream());
        }

        [Test]
        public void CreatesIntervalIfCloseIsMissing()
        {
            var sut = new FileOpenTransformer(_context);

            sut.ProcessEvent(TestDocumentEvent(0, 1, "File1.cs", true, "a"));
            // missing close!

            var expected = new[]
            {
                ExpectedInterval(0, 1, "File1.cs", "a")
            };

            CollectionAssert.AreEquivalent(expected, sut.SignalEndOfEventStream());
        }
    }
}
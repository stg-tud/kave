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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    internal class CloseEventProcessorTest
    {
        private CloseEventProcessor _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new CloseEventProcessor();
        }

        [Test]
        public void ShouldNotFilterOtherEvents()
        {
            ProcessorAssert.DoesNotFilter(_uut, new TestIDEEvent());
        }

        [Test]
        public void ShouldNotFilterNonClosingDocumentEvents()
        {
            ProcessorAssert.DoesNotFilter(_uut, new DocumentEvent {Action = DocumentAction.Opened});
        }

        [Test]
        public void ReplacesDocumentClosingEventWithCloseEventTest()
        {
            var closingEvent = new DocumentEvent {Action = DocumentAction.Closing};

            var resultSet = _uut.Map(closingEvent);

            var expected = new CommandEvent {CommandId = "Close"};
            CollectionAssert.AreEquivalent(new[] {expected}, resultSet);
        }

        [Test]
        public void FiltersWindowCloseEvents()
        {
            var windowClosingEvent = new WindowEvent {Action = WindowAction.Close};

            var processedEvent = _uut.Map(windowClosingEvent);

            CollectionAssert.IsEmpty(processedEvent);
        }

        [Test]
        public void ShouldNotFilterNonClosingWindowEvents()
        {
            ProcessorAssert.DoesNotFilter(_uut, new WindowEvent {Action = WindowAction.Activate});
        }

        [Test]
        public void FiltersDocumentCloseAfterCommandClose()
        {
            var commandCloseEvent = new CommandEvent {CommandId = "Close"};
            _uut.Map(commandCloseEvent);

            var documentEvent = new DocumentEvent {Action = DocumentAction.Closing};
            var processedEvent = _uut.Map(documentEvent);

            CollectionAssert.IsEmpty(processedEvent);
        }
    }
}
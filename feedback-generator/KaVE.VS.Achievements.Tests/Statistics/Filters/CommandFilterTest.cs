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
using KaVE.VS.Achievements.Statistics.Filters;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Statistics.Filters
{
    [TestFixture]
    public class CommandFilterTest
    {
        [SetUp]
        public void Init()
        {
            _commandFilter = new CommandFilter();
        }

        private CommandFilter _commandFilter;

        private static readonly IDEEvent[][] CloseEvents =
        {
            new IDEEvent[]
            {
                new CommandEvent {CommandId = "File.Close"},
                new CommandEvent {CommandId = "Close"}
            }
        };

        [Test]
        public void DoesntFilterCommitClickEvents()
        {
            var commitClickEvent = new CommandEvent {TriggeredBy = IDEEvent.Trigger.Click, CommandId = "Comm_it"};
            Assert.AreEqual(commitClickEvent, _commandFilter.Process(commitClickEvent));
        }

        [Test]
        public void FiltersClickEventsTest()
        {
            var clickEvent = new CommandEvent {TriggeredBy = IDEEvent.Trigger.Click, CommandId = "ClickEvent"};
            Assert.IsNull(_commandFilter.Process(clickEvent));
        }

        [Test]
        public void FiltersEventsWithoutCommandId()
        {
            var clickEvent = new CommandEvent();
            Assert.IsNull(_commandFilter.Process(clickEvent));
        }

        [Test, TestCaseSource("CloseEvents")]
        public void FiltersCloseEvents(IDEEvent[] @events)
        {
            foreach (var @event in @events)
            {
                Assert.IsNull(_commandFilter.Process(@event));
            }
        }

        [Test]
        public void FiltersOtherDocumentEventsTest()
        {
            var nonClosingEvent = new DocumentEvent();

            Assert.IsNull(_commandFilter.Process(nonClosingEvent));
        }

        [Test]
        public void ProcessesCommandEventsCorrectly()
        {
            var publishedEvent = new CommandEvent {CommandId = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start"};
            Assert.AreEqual(publishedEvent, _commandFilter.Process(publishedEvent));
        }

        [Test]
        public void ReplacesDocumentClosingEventWithCloseEventTest()
        {
            var closingEvent = new DocumentEvent {Action = DocumentEvent.DocumentAction.Closing};

            var actualEvent = (CommandEvent) _commandFilter.Process(closingEvent);

            const string expectedId = "Close";
            Assert.AreEqual(expectedId, actualEvent.CommandId);
        }
    }
}
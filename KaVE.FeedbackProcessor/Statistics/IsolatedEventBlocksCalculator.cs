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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class IsolatedEventBlocksCalculator : BaseEventProcessor
    {
        public readonly TimeSpan MaximumBlockSpan;
        public readonly TimeSpan LongBreak;

        public readonly IKaVEList<IKaVEList<IDEEvent>> LoggedIsolatedBlocks = Lists.NewList<IKaVEList<IDEEvent>>();

        private IDEEvent _lastEvent;

        private bool _weMightBeInAnIsolatedBlock;

        private readonly IList<IDEEvent> _loggedEvents = new List<IDEEvent>();

        public IsolatedEventBlocksCalculator(TimeSpan longBreak, TimeSpan maximumBlockSpan)
        {
            LongBreak = longBreak;
            MaximumBlockSpan = maximumBlockSpan;

            RegisterFor<IDEEvent>(LogIsolatedEventBlocks);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _lastEvent = null;
            _weMightBeInAnIsolatedBlock = false;
            _loggedEvents.Clear();
        }

        private void LogIsolatedEventBlocks(IDEEvent currentEvent)
        {
            if (WeWereAnIsolatedBlockAndLeftItWith(currentEvent))
            {
                LogFilteredBlock(_loggedEvents);
                _loggedEvents.Clear();
                _weMightBeInAnIsolatedBlock = false;
            }

            if (ALargeBreakOccured(currentEvent))
            {
                _weMightBeInAnIsolatedBlock = true;
            }

            if (_weMightBeInAnIsolatedBlock)
            {
                _loggedEvents.Add(currentEvent);

                if (TheBlockIsNotIsolated(currentEvent))
                {
                    _loggedEvents.Clear();
                    _weMightBeInAnIsolatedBlock = false;
                }
            }

            _lastEvent = currentEvent;
        }

        private bool WeWereAnIsolatedBlockAndLeftItWith(IDEEvent currentEvent)
        {
            return _weMightBeInAnIsolatedBlock && GetTimeDifference(currentEvent, _lastEvent) >= LongBreak;
        }

        private bool TheBlockIsNotIsolated(IDEEvent currentEvent)
        {
            return _loggedEvents.Count > 0 &&
                   GetTimeDifference(_loggedEvents.First(), currentEvent) > MaximumBlockSpan;
        }

        private bool ALargeBreakOccured(IDEEvent currentEvent)
        {
            return _weMightBeInAnIsolatedBlock == false && _lastEvent != null &&
                   GetTimeDifference(_lastEvent, currentEvent) >= LongBreak;
        }

        private static TimeSpan GetTimeDifference(IDEEvent event1, IDEEvent event2)
        {
            return event2.GetTriggeredAt() > event1.GetTriggeredAt()
                ? event2.GetTriggeredAt() - event1.GetTriggeredAt()
                : event1.GetTriggeredAt() - event2.GetTriggeredAt();
        }

        private void LogFilteredBlock(IEnumerable<IDEEvent> filteredIsolatedEventBlock)
        {
            LoggedIsolatedBlocks.Add(Lists.NewListFrom(filteredIsolatedEventBlock));
        }

        public string LoggedIsolatedBlocksToTxt()
        {
            var stringBuilder = new StringBuilder();

            foreach (var filteredIsolatedBlock in LoggedIsolatedBlocks)
            {
                stringBuilder.AppendLine("--- Start of isolated block ---");
                filteredIsolatedBlock.ToList()
                                     .ForEach(
                                         eventFromBlock =>
                                             stringBuilder.AppendLine(
                                                 "\t" + EventMappingUtils.GetAbstractStringOf(eventFromBlock)));
                stringBuilder.AppendLine("--- End of isolated block ---");
                stringBuilder.AppendLine("");
            }

            return stringBuilder.ToString();
        }
    }
}
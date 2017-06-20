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

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class IsolatedEventBlockFilter : BaseEventMapper
    {
        public readonly TimeSpan MaximumBlockSpan;

        public readonly TimeSpan LongBreak;

        public readonly IKaVEList<IKaVEList<IDEEvent>> FilteredIsolatedBlocks = Lists.NewList<IKaVEList<IDEEvent>>();

        private IDEEvent _lastEvent;

        private bool _weMightBeInAnIsolatedBlock;

        private readonly IList<IDEEvent> _droppedEvents = new List<IDEEvent>();

        public IsolatedEventBlockFilter(TimeSpan longBreak, TimeSpan maximumBlockSpan)
        {
            LongBreak = longBreak;
            MaximumBlockSpan = maximumBlockSpan;

            RegisterFor<IDEEvent>(FilterIsolatedEventBlocks);
        }

        private void FilterIsolatedEventBlocks(IDEEvent currentEvent)
        {
            if (WeWereAnIsolatedBlockAndLeftItWith(currentEvent))
            {
                LogFilteredBlock(_droppedEvents);
                _droppedEvents.Clear();
                _weMightBeInAnIsolatedBlock = false;
            }

            if (ALargeBreakOccured(currentEvent))
            {
                _weMightBeInAnIsolatedBlock = true;
            }

            if (_weMightBeInAnIsolatedBlock)
            {
                DropCurrentEvent();
                _droppedEvents.Add(currentEvent);

                if (TheBlockIsNotIsolated(currentEvent))
                {
                    Insert(_droppedEvents.ToArray());
                    _droppedEvents.Clear();
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
            return _droppedEvents.Count > 0 &&
                   GetTimeDifference(_droppedEvents.First(), currentEvent) > MaximumBlockSpan;
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
            FilteredIsolatedBlocks.Add(Lists.NewListFrom(filteredIsolatedEventBlock));
        }

        public string FilteredIsolatedBlocksToTxt()
        {
            var stringBuilder = new StringBuilder();

            foreach (var filteredIsolatedBlock in FilteredIsolatedBlocks)
            {
                stringBuilder.AppendLine("--- Start of filtered block ---");
                filteredIsolatedBlock.ToList()
                                     .ForEach(
                                         eventFromBlock =>
                                             stringBuilder.AppendLine(
                                                 "\t" + EventMappingUtils.GetAbstractStringOf(eventFromBlock)));
                stringBuilder.AppendLine("--- End of filtered block ---");
                stringBuilder.AppendLine("");
            }

            return stringBuilder.ToString();
        }
    }
}
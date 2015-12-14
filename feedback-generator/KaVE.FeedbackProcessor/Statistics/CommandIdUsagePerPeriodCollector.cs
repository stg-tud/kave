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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class CommandIdUsagePerPeriodCollector : BaseEventProcessor
    {
        private readonly TimeSpan _periodDuration;
        private readonly TimeSpan _developerDayOffset;

        private DateTime? _periodStart;
        private readonly ISet<string> _commandIdsInPeriod = new HashSet<string>();
        public readonly IMultiset<string> CommandIdsWithPeriodUsageFrequency = new Multiset<string>();
        public int NumberOfPeriods { get; private set; }

        public CommandIdUsagePerPeriodCollector(TimeSpan periodDuration, TimeSpan developerDayOffset)
        {
            _periodDuration = periodDuration;
            RegisterFor<CommandEvent>(HandleCommandEvent);
            RegisterFor<CompletionEvent>(HandleCompletionEvent);
            _developerDayOffset = developerDayOffset;
        }

        private void HandleCommandEvent(CommandEvent @event)
        {
            HandleEvent(@event, @event.CommandId);
        }

        private void HandleCompletionEvent(CompletionEvent @event)
        {
            HandleEvent(@event, "KaVE$Completion_" + @event.TerminatedState);
            HandleEvent(@event, "KaVE$Completion_All");
        }

        private void HandleEvent(IDEEvent @event, string commandId)
        {
            if (_periodStart == null)
            {
                _periodStart = @event.GetTriggerDate() + _developerDayOffset;
            }

            while (_periodStart + _periodDuration < @event.GetTriggeredAt())
            {
                ClosePeriod();
                _periodStart += _periodDuration;
            }

            _commandIdsInPeriod.Add(commandId);
        }

        public override void OnStreamEnds()
        {
            ClosePeriod();
            _periodStart = null;
        }

        private void ClosePeriod()
        {
            if (_commandIdsInPeriod.Any())
            {
                NumberOfPeriods++;
            }

            foreach (var commandId in _commandIdsInPeriod)
            {
                CommandIdsWithPeriodUsageFrequency.Add(commandId);
            }

            _commandIdsInPeriod.Clear();
        }
    }
}
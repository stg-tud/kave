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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class ManualCompletionsStatistic : BaseEventProcessor
    {
        private static readonly CommandEvent NoTrigger = new CommandEvent {TriggeredAt = new DateTime()};
        private static readonly CompletionEvent NoCompletion = new CompletionEvent {TriggeredAt = new DateTime()};

        private CommandEvent _manualCodeCompletionTrigger = NoTrigger;
        private CompletionEvent _filteredCompletion = NoCompletion;

        public int NumberOfManualCompletions { get; private set; }
        public TimeSpan DurationInManualCompletion = TimeSpan.Zero;
        public int NumberOfDeveloperDaysWithManualCompletionUsage { get; private set; }
        public int NumberOfAppliedCompletions { get; private set; }
        public int NumberOfCancelledCompletions { get; private set; }

        private DateTime _today;
        private bool _developerCountedToday;

        public ManualCompletionsStatistic()
        {
            RegisterFor<CommandEvent>(OnCommandEvent);
            RegisterFor<CompletionEvent>(OnCompletionEvent);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _today = new DateTime();
            _developerCountedToday = false;
        }

        private void OnCompletionEvent(CompletionEvent @event)
        {
            if (Difference(_manualCodeCompletionTrigger, @event) < TimeSpan.FromSeconds(1) ||
                Difference(_filteredCompletion, @event) < TimeSpan.FromSeconds(1))
            {
                // ReSharper disable once PossibleInvalidOperationException
                DurationInManualCompletion += @event.Duration.Value;

                if (@event.TerminatedState == TerminationState.Filtered)
                {
                    _filteredCompletion = @event;
                }
                else
                {
                    if (@event.TerminatedState == TerminationState.Applied)
                    {
                        NumberOfAppliedCompletions++;
                    }
                    else if (@event.TerminatedState == TerminationState.Cancelled)
                    {
                        NumberOfCancelledCompletions++;
                    }
                    _filteredCompletion = NoCompletion;
                }
                _manualCodeCompletionTrigger = NoTrigger;
            }
        }

        private static TimeSpan Difference(IDEEvent firstEvent, IDEEvent secondEvent)
        {
            return (secondEvent.GetTriggeredAt() - firstEvent.GetTriggeredAt());
        }

        private void OnCommandEvent(CommandEvent @event)
        {
            if ("CompleteCodeBasic".Equals(@event.CommandId))
            {
                NumberOfManualCompletions++;
                _manualCodeCompletionTrigger = @event;

                var date = @event.GetTriggerDate();
                if (!_developerCountedToday && date != _today)
                {
                    _today = date;
                    NumberOfDeveloperDaysWithManualCompletionUsage++;
                }
            }
        }
    }
}
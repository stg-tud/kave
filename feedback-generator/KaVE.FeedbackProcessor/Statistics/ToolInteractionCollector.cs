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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Properties;

namespace KaVE.FeedbackProcessor.Statistics
{
    class ToolInteractionCollector : BaseEventProcessor
    {
        private const string IDECore = "IDE Core";
        private readonly ISet<string> _developerDaysToFilter;
        private readonly CsvTable _commandToTool;

        private readonly IDictionary<DeveloperDay, IMultiset<string>> _toolUsagesPerDay = new Dictionary<DeveloperDay, IMultiset<string>>();
        private readonly TimeSpan _developerDayOffset;
        private Developer _currentDeveloper;
        private DeveloperDay _lastDeveloperDay;
        private string _lastToolUsed;

        public ToolInteractionCollector(TimeSpan developerDayOffset)
        {
            _commandToTool = CsvTable.Read(Resources.CommandToolMapping, ';');
            _developerDaysToFilter = new HashSet<string>(CsvTable.Read(Resources.ExcludedDays).Rows.Select(row => row["developer day"]));
            _developerDayOffset = developerDayOffset;
            RegisterFor<CommandEvent>(HandleCommandEvent);
            RegisterFor<CompletionEvent>(HandleCompletionEvent);
            RegisterFor<WindowEvent>(HandleWindowEvent);
        }

        private void HandleWindowEvent(WindowEvent @event)
        {
            if (@event.Window.Caption.Contains("NCrunch"))
            {
                HandleEvent(@event, "NCrunch");
            }
        }

        public override void OnStreamStarts(Developer developer)
        {
            _currentDeveloper = developer;
        }

        private void HandleCommandEvent(CommandEvent @event)
        {
            HandleEvent(@event, GetCorrespondingTool(@event));
        }

        private void HandleCompletionEvent(CompletionEvent @event)
        {
            HandleEvent(@event, "Completion");
            if (@event.TerminatedState == TerminationState.Applied)
            {
                HandleEvent(@event, "Completion-Successful");
            }
        }

        private void HandleEvent(IDEEvent @event, string tool)
        {
            var developerDay = GetDeveloperDay(@event);

            if (_developerDaysToFilter.Contains(developerDay.Id))
            {
                return;
            }

            if (IsNewInteraction(developerDay, tool))
            {
                if (!_toolUsagesPerDay.ContainsKey(developerDay))
                {
                    _toolUsagesPerDay[developerDay] = new Multiset<string>();
                }

                _toolUsagesPerDay[developerDay].Add(tool);
                _lastDeveloperDay = developerDay;
                _lastToolUsed = tool;
            }
        }

        private bool IsNewInteraction(DeveloperDay developerDay, string tool)
        {
            var isNewDay = !developerDay.Equals(_lastDeveloperDay);
            var isDifferentTool = !tool.Equals(_lastToolUsed);
            var isNotCore = !tool.Equals(IDECore);
            return isNewDay || (isDifferentTool && isNotCore);
        }

        private DeveloperDay GetDeveloperDay(IDEEvent @event)
        {
            return new DeveloperDay(_currentDeveloper, GetDeveloperDayDate(@event));
        }

        private DateTime GetDeveloperDayDate(IDEEvent @event)
        {
            try
            {
                return (@event.GetTriggeredAt() - _developerDayOffset).Date;
            }
            catch (ArgumentOutOfRangeException)
            {
                return @event.GetTriggerDate();
            }
        }

        private string GetCorrespondingTool(CommandEvent @event)
        {
            var commandId = @event.CommandId;
            var candidate = _commandToTool.Rows.FirstOrDefault(row => commandId.StartsWith(row["Command Id"]));
            if (candidate == null)
            {
                return IDECore;
            }
            if (IsStaticAnalysisTab(commandId))
            {
                return "StaticAnalysis";
            }
            if (Regex.IsMatch(commandId, @"^\d+ "))
            {
                return IDECore;
            }
            return candidate["Tool"];
        }

        private static bool IsStaticAnalysisTab(string commandId)
        {
            return commandId.EndsWith("Error") || commandId.EndsWith("Errors") || commandId.EndsWith("Warning") || commandId.EndsWith("Warnings") || commandId.EndsWith("Message") || commandId.EndsWith("Messages");
        }

        public string UsagesToCsv()
        {
            var builder = new CsvBuilder();
            foreach (var usagesForDay in _toolUsagesPerDay)
            {
                builder.StartRow();
                builder["developer"] = usagesForDay.Key.Id;
                foreach (var usages in usagesForDay.Value.EntryDictionary)
                {
                    builder[usages.Key] = usages.Value;
                }
            }
            return builder.Build();
        }
    }
}
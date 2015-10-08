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
using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Exceptions;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.Statistics;

namespace KaVE.VS.Statistics.Calculators
{
    [ShellComponent]
    public class GlobalCalculator : StatisticCalculator<GlobalStatistic>
    {
        public static readonly TimeSpan SessionTimeOut = new TimeSpan(0, 2, 0);

        private DateTime? _debugStartTime;

        private DateTime? _lastEventTime;

        public GlobalCalculator(IStatisticListing statisticListing, IMessageBus messageBus, ILogger errorHandler)
            : base(statisticListing, messageBus, errorHandler) {}

        protected override void Calculate(GlobalStatistic statistic, IDEEvent @event)
        {
            statistic.TotalEvents++;

            if (statistic.EarliestEventTime == null ||
                (@event.TriggeredAt != null &&
                 @event.TriggeredAt.Value.TimeOfDay < statistic.EarliestEventTime.Value.TimeOfDay))
            {
                statistic.EarliestEventTime = @event.TriggeredAt;
            }

            if (statistic.LatestEventTime == null ||
                (@event.TriggeredAt != null &&
                 @event.TriggeredAt.Value.TimeOfDay > statistic.LatestEventTime.Value.TimeOfDay))
            {
                statistic.LatestEventTime = @event.TriggeredAt;
            }

            CalculateTotalWorkTime(@event, statistic);

            var editEvent = @event as EditEvent;
            if (editEvent != null)
            {
                Process(editEvent, statistic);
            }

            var commandEvent = @event as CommandEvent;
            if (commandEvent != null)
            {
                Process(commandEvent, statistic);
            }

            var debuggerEvent = @event as DebuggerEvent;
            if (debuggerEvent != null)
            {
                Process(debuggerEvent, statistic);
            }

            _lastEventTime = @event.TriggeredAt;
        }

        private void CalculateTotalWorkTime(IDEEvent @event, GlobalStatistic globalStatistic)
        {
            if (@event.TriggeredAt == null || _lastEventTime == null)
            {
                return;
            }

            var timeSpan = @event.TriggeredAt.Value.Subtract(_lastEventTime.Value);
            if (@event.TriggeredAt.Value > _lastEventTime.Value && timeSpan < SessionTimeOut)
            {
                globalStatistic.TotalWorkTime += timeSpan;
            }
        }

        private static void Process(EditEvent editEvent, GlobalStatistic globalStatistic)
        {
            globalStatistic.CurrentNumberOfEditsBetweenCommits += editEvent.NumberOfChanges;
            globalStatistic.TotalNumberOfEdits += editEvent.NumberOfChanges;
        }

        private static void Process(CommandEvent commandEvent, GlobalStatistic globalStatistic)
        {
            if (commandEvent.CommandId != "Commit" &&
                commandEvent.CommandId != "Commit and Push" &&
                commandEvent.CommandId != "Commit and Sync" &&
                commandEvent.CommandId != "Comm_it")
            {
                return;
            }

            if (globalStatistic.CurrentNumberOfEditsBetweenCommits > globalStatistic.MaxNumberOfEditsBetweenCommits)
            {
                globalStatistic.MaxNumberOfEditsBetweenCommits = globalStatistic.CurrentNumberOfEditsBetweenCommits;
            }

            globalStatistic.CurrentNumberOfEditsBetweenCommits = 0;
        }

        private void Process(DebuggerEvent debuggerEvent, GlobalStatistic globalStatistic)
        {
            if (_debugStartTime == null &&
                (debuggerEvent.Reason == "dbgEventReasonLaunchProgram" ||
                 debuggerEvent.Reason == "dbgEventReasonGo" ||
                 debuggerEvent.Reason == "dbgEventReasonAttachProgram"))
            {
                _debugStartTime = debuggerEvent.TriggeredAt;
            }

            if (debuggerEvent.Reason != "dbgEventReasonStopDebugging" &&
                debuggerEvent.Reason != "dbgEventReasonEndProgram" &&
                debuggerEvent.Reason != "dbgEventReasonDetachProgram")
            {
                return;
            }

            var debugEndTime = debuggerEvent.TriggeredAt;
            if (debugEndTime != null && _debugStartTime != null)
            {
                var timeInDebugSession = debugEndTime.Value.Ticks - _debugStartTime.Value.Ticks;
                if (globalStatistic.TimeInDebugSession.Ticks < timeInDebugSession)
                {
                    globalStatistic.TimeInDebugSession = new TimeSpan(timeInDebugSession);
                }
            }
            _debugStartTime = null;
        }
    }
}
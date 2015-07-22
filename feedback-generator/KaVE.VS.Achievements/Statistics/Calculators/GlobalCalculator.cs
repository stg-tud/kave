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
using KaVE.VS.Achievements.Statistics.Calculators.BaseClasses;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.Achievements.Statistics.Calculators
{
    [ShellComponent]
    public class GlobalCalculator : StatisticCalculator
    {
        public static readonly TimeSpan SessionTimeOut = new TimeSpan(0, 2, 0);

        private DateTime? _debugStartTime;

        private DateTime? _lastEventTime;

        public GlobalCalculator(IStatisticListing statisticListing, IMessageBus messageBus, IErrorHandler errorHandler)
            : base(statisticListing, messageBus, errorHandler, typeof (GlobalStatistic)) {}

        protected override IStatistic Process(IDEEvent @event)
        {
            var globalStatistic = (GlobalStatistic) StatisticListing.GetStatistic(StatisticType);

            globalStatistic.TotalEvents++;

            if (@event.TriggeredAt != null &&
                @event.TriggeredAt.Value.TimeOfDay < globalStatistic.EarliestEventTime.TimeOfDay)
            {
                globalStatistic.EarliestEventTime = @event.TriggeredAt.Value;
            }

            if (@event.TriggeredAt != null &&
                @event.TriggeredAt.Value.TimeOfDay > globalStatistic.LatestEventTime.TimeOfDay)
            {
                globalStatistic.LatestEventTime = @event.TriggeredAt.Value;
            }

            CalculateTotalWorkTime(@event, globalStatistic);

            var editEvent = @event as EditEvent;
            if (editEvent != null)
            {
                Process(editEvent, globalStatistic);
            }

            var commandEvent = @event as CommandEvent;
            if (commandEvent != null)
            {
                Process(commandEvent, globalStatistic);
            }

            var debuggerEvent = @event as DebuggerEvent;
            if (debuggerEvent != null)
            {
                Process(debuggerEvent, globalStatistic);
            }

            _lastEventTime = @event.TriggeredAt;

            return globalStatistic;
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
﻿/*
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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    public class TransformerContext : IEventToIntervalTransformer<Interval>
    {
        public string CurrentProject { get; set; }

        public TransformerContext()
        {
            CurrentProject = string.Empty;
        }

        public TIntervalType CreateIntervalFromEvent<TIntervalType>(IDEEvent ideEvent)
            where TIntervalType : Interval, new()
        {
            return new TIntervalType
            {
                StartTime = ideEvent.TriggeredAt.GetValueOrDefault(),
                Duration = ideEvent.Duration.GetValueOrDefault(),
                IDESessionId = ideEvent.IDESessionUUID,
                Project = CurrentProject
            };
        }

        public void AdaptIntervalTimeData(Interval interval, IDEEvent ideEvent)
        {
            if (ideEvent.TerminatedAt.GetValueOrDefault() > interval.StartTime + interval.Duration)
            {
                interval.Duration = ideEvent.TerminatedAt.GetValueOrDefault() - interval.StartTime;
            }

            if (interval.Project == string.Empty)
            {
                interval.Project = CurrentProject;
            }
        }

        public void ProcessEvent(IDEEvent @event)
        {
            var se = @event as SolutionEvent;
            if (se != null && se.Action == SolutionEvent.SolutionAction.OpenSolution)
            {
                CurrentProject = se.Target.Identifier;
            }
        }

        public IEnumerable<Interval> SignalEndOfEventStream()
        {
            return Enumerable.Empty<Interval>();
        }
    }
}
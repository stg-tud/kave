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
 * 
 * Contributors:
 *    - Mattis Manfred Kämmerer
 *    - Markus Zimmermann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class ConcurrentEventProcessor : BaseEventMapper
    {
        public static TimeSpan EventTimeDifference = new TimeSpan(0, 0, 0, 0, 1);

        private readonly IList<IDEEvent> _eventCache;

        public ConcurrentEventProcessor()
        {
            _eventCache = new List<IDEEvent>();

            RegisterFor<IDEEvent>(ProcessIDEEvent);
        }

        private void ProcessIDEEvent(IDEEvent @event)
        {
            if (@event is ErrorEvent)
            {
                DropCurrentEvent();
                return;
            }

            if (_eventCache.Count == 0)
            {
                _eventCache.Add(@event);
            }
            else
            {
                var lastEventTime = _eventCache.Last().TriggeredAt;
                var currentEventTime = @event.TriggeredAt;

                if (ConcurrentEventHeuristic.HaveSimiliarEventTime(currentEventTime, lastEventTime, EventTimeDifference))
                {
                    _eventCache.Add(@event);
                }
                else
                {
                    var concurrentEvent = GenerateConcurrentEvent();
                    _eventCache.Clear();
                    _eventCache.Add(@event);
                    if (MoreThanOneEventConcurredIn(concurrentEvent))
                    {
                        ReplaceCurrentEventWith(concurrentEvent);
                        return;
                    }
                }
            }

            DropCurrentEvent();
        }

        private static bool MoreThanOneEventConcurredIn(ConcurrentEvent concurrentEvent)
        {
            return concurrentEvent.ConcurrentEventList.Count > 1;
        }

        private ConcurrentEvent GenerateConcurrentEvent()
        {
            return new ConcurrentEvent
            {
                ConcurrentEventList = new List<IDEEvent>(_eventCache),
                IDESessionUUID = _eventCache.First().IDESessionUUID,
                Id = _eventCache.First().Id,
                TriggeredAt = _eventCache.First().TriggeredAt,
                TriggeredBy = _eventCache.First().TriggeredBy,
                TerminatedAt = _eventCache.Last().TerminatedAt,
                Duration = _eventCache.Last().TerminatedAt - _eventCache.First().TriggeredAt,
                KaVEVersion = _eventCache.First().KaVEVersion
            };
        }
    }
}
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
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    internal class UserActiveTransformer : IEventToIntervalTransformer<UserActiveInterval>
    {
        private readonly IList<UserActiveInterval> _intervals;
        private readonly TimeSpan _maxInactivityTime;

        public UserActiveTransformer(TimeSpan? maxInactivityTime = null)
        {
            _intervals = new List<UserActiveInterval>();
            _maxInactivityTime = maxInactivityTime ?? TimeSpan.FromSeconds(16);
        }

        public void ProcessEvent(IDEEvent e)
        {
            var lastInterval = _intervals.LastOrDefault();
            var currentEventStartTime = e.TriggeredAt.GetValueOrDefault();
            var currentEventEndTime = e.TerminatedAt.GetValueOrDefault();

            if (lastInterval != null && lastInterval.EndTime +_maxInactivityTime >= currentEventStartTime)
            {
                lastInterval.Duration = currentEventEndTime - lastInterval.StartTime;
            }
            else
            {
                _intervals.Add(TransformerUtils.CreateIntervalFromFirstEvent<UserActiveInterval>(e));
            }
        }

        public IEnumerable<UserActiveInterval> SignalEndOfEventStream()
        {
            return _intervals;
        }
    }
}
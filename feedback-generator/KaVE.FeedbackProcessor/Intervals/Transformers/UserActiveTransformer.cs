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

using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    internal class UserActiveTransformer : SingleIntervalTransformerBase<UserActiveInterval>
    {
        private const int MaxInactivityTime = 16;

        public override void OnEvent(IDEEvent e)
        {
            if (EventHasNoTimeData(e))
            {
                return;
            }

            if (_currentInterval != null)
            {
                var currentIntervalEndTime = _currentInterval.StartTime + _currentInterval.Duration;
                var currentEventStartTime = e.TriggeredAt.GetValueOrDefault();
                var currentEventEndTime = e.TerminatedAt.GetValueOrDefault();

                if (currentIntervalEndTime.AddSeconds(MaxInactivityTime) < currentEventStartTime)
                {
                    FireInterval();
                }
                else
                {
                    if (currentIntervalEndTime < currentEventEndTime)
                    {
                        _currentInterval.Duration = currentEventEndTime - _currentInterval.StartTime;
                    }
                }
            }

            if (_currentInterval == null)
            {
                CreateIntervalFromFirstEvent(e);
            }
        }

        public override void OnStreamEnds()
        {
            FireInterval();
        }
    }
}
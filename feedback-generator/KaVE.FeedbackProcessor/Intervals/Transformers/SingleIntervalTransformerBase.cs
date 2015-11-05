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
using KaVE.FeedbackProcessor.Intervals.Model;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    internal abstract class SingleIntervalTransformerBase<T> : IEventToIntervalTransformer<T>
        where T : Interval, new()
    {
        public virtual event IntervalCompletedHandler<T> IntervalCompleted;

        protected T _currentInterval;
        protected DateTime _lastEventTime;

        protected bool EventHasNoTimeData(IDEEvent ideEvent)
        {
            return !ideEvent.TriggeredAt.HasValue || !ideEvent.TerminatedAt.HasValue;
        }

        protected void CreateIntervalFromFirstEvent(IDEEvent ideEvent)
        {
            _currentInterval = new T
            {
                StartTime = ideEvent.TriggeredAt.GetValueOrDefault(),
                Duration = ideEvent.Duration.GetValueOrDefault()
            };
        }

        protected void FireInterval()
        {
            if (_currentInterval == null)
            {
                return;
            }

            var handlers = IntervalCompleted;
            if (handlers != null)
            {
                handlers(_currentInterval);
            }

            _currentInterval = null;
        }

        public virtual void OnStreamStarts(Developer developer) {}

        public virtual void OnEvent(IDEEvent @event) {}

        public virtual void OnStreamEnds() {}
    }
}
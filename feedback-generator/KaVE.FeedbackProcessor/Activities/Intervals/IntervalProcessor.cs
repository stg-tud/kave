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
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Activities.Intervals
{
    internal abstract class IntervalProcessor<T> : BaseEventProcessor
    {
        public IDictionary<Developer, IntervalStream<T>> Intervals = new Dictionary<Developer, IntervalStream<T>>();
        private Developer _currentDeveloper;
        protected Interval<T> CurrentInterval { get; private set; }

        protected IntervalProcessor()
        {
            RegisterFor<ActivityEvent>(Handle);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _currentDeveloper = developer;
            Intervals[developer] = new IntervalStream<T>();
            CurrentInterval = null;
        }

        private void Handle(ActivityEvent @event)
        {
            CountEvent(@event);
            UpdateIntervals(@event);
        }

        private void CountEvent(ActivityEvent @event)
        {
            Intervals[_currentDeveloper].TotalNumberOfActivities++;
            if (@event.Activity == Activity.Any)
            {
                Intervals[_currentDeveloper].NumberOfAnyActivities++;
            }
        }

        private void UpdateIntervals(ActivityEvent @event)
        {
            if (HasNoOpenInterval())
            {
                StartInterval(@event);
            }
            else
            {
                HandleWithCurrentInterval(@event);
            }
        }

        private bool HasNoOpenInterval()
        {
            return CurrentInterval == null;
        }

        protected void StartInterval(ActivityEvent @event)
        {
            StartInterval(@event.GetTriggeredAt(), GetIntervalId(@event), GetEnd(@event));
        }

        protected void StartInterval(DateTime start, T activity, DateTime end)
        {
            Asserts.That(start <= end, "interval ends before it starts");
            CurrentInterval = new Interval<T>
            {
                Start = start,
                Id = activity,
                End = end
            };
            Intervals[_currentDeveloper].Append(CurrentInterval);
        }

        protected abstract T GetIntervalId(ActivityEvent @event);

        protected static DateTime GetEnd(ActivityEvent @event)
        {
            if (@event.TerminatedAt != null && @event.Duration > TimeSpan.Zero)
            {
                return (DateTime) @event.TerminatedAt;
            }
            return @event.GetTriggeredAt();
        }

        protected abstract void HandleWithCurrentInterval(ActivityEvent @event);
    }
}
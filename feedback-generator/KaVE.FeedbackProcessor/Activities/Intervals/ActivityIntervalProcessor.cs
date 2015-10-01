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
using KaVE.Commons.Utils;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Activities.Intervals
{
    internal class ActivityIntervalProcessor : BaseEventProcessor
    {
        public class Interval
        {
            public Activity Activity { get; internal set; }
            public DateTime Start { get; internal set; }
            public DateTime End { get; internal set; }

            protected bool Equals(Interval other)
            {
                return Activity == other.Activity && Start.Equals(other.Start) && End.Equals(other.End);
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj, Equals);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (int) Activity;
                    hashCode = (hashCode*397) ^ Start.GetHashCode();
                    hashCode = (hashCode*397) ^ End.GetHashCode();
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return string.Format("Start: {1}, Activity: {0}, End: {2}", Activity, Start, End);
            }
        }

        public IDictionary<Developer, IList<Interval>> Intervals;
        private Developer _currentDeveloper;
        private Interval _currentInterval;
        private readonly TimeSpan _activityTimeout;

        public ActivityIntervalProcessor()
        {
            Intervals = new Dictionary<Developer, IList<Interval>>();
            RegisterFor<ActivityEvent>(Handle);
            _activityTimeout = TimeSpan.FromMilliseconds(16000);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _currentDeveloper = developer;
            Intervals[developer] = new List<Interval>();
            _currentInterval = null;
        }

        private void Handle(ActivityEvent @event)
        {
            if (HasNoOpenInterval())
            {
                StartIntervalAt(@event);
            }

            if (_currentInterval.Activity != @event.Activity)
            {
                StartIntervalAt(@event);
            }
            else
            {
                _currentInterval.End = GetEnd(@event);
            }
        }

        private static DateTime GetEnd(ActivityEvent @event)
        {
            return @event.TerminatedAt ?? @event.GetTriggeredAt();
        }

        private bool HasNoOpenInterval()
        {
            return _currentInterval == null;
        }

        private void StartIntervalAt(ActivityEvent @event)
        {
            _currentInterval = new Interval
            {
                Start = @event.GetTriggeredAt(),
                Activity = @event.Activity,
                End = GetEnd(@event)
            };
            Intervals[_currentDeveloper].Add(_currentInterval);
        }

        public void CorrectIntervalsWithTimeout(TimeSpan fromSeconds)
        {
            foreach (var intervalStream in Intervals.Values)
            {
                Interval previousInterval = null;
                foreach (var interval in intervalStream)
                {
                    if (previousInterval != null)
                    {
                        previousInterval.End = interval.Start;
                    }

                    previousInterval = interval;
                }
            }
        }
    }
}
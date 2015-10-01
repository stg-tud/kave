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
                StartInterval(@event);
            }

            if (EndsAwayButNotInAwayInterval(@event))
            {
                StartInterval(_currentInterval.End, Activity.Away, @event.GetTriggeredAt());
            }

            if (EndsCurrentInterval(@event))
            {
                var previousInterval = _currentInterval;

                StartInterval(@event);

                if (previousInterval.Activity == Activity.Away)
                {
                    previousInterval.End = _currentInterval.Start;
                }
            }
            else
            {
                _currentInterval.End = GetEnd(@event);
            }
        }

        private bool HasNoOpenInterval()
        {
            return _currentInterval == null;
        }

        private void StartInterval(ActivityEvent @event)
        {
            StartInterval(@event.GetTriggeredAt(), GetIntervalActivity(@event), GetEnd(@event));
        }

        private static DateTime GetEnd(ActivityEvent @event)
        {
            return @event.TerminatedAt ?? @event.GetTriggeredAt();
        }

        private static Activity GetIntervalActivity(ActivityEvent @event)
        {
            switch (@event.Activity)
            {
                case Activity.LeaveIDE:
                    return Activity.Away;
                case Activity.EnterIDE:
                    return Activity.Other;
                default:
                    return @event.Activity;
            }
        }

        private void StartInterval(DateTime start, Activity activity, DateTime end)
        {
            _currentInterval = new Interval
            {
                Start = start,
                Activity = activity,
                End = end
            };
            Intervals[_currentDeveloper].Add(_currentInterval);
        }

        private bool EndsAwayButNotInAwayInterval(ActivityEvent @event)
        {
            return @event.Activity == Activity.EnterIDE && _currentInterval.Activity != Activity.Away;
        }

        private bool EndsCurrentInterval(ActivityEvent @event)
        {
            return _currentInterval.Activity != GetIntervalActivity(@event);
        }

        public void CorrectIntervalsWithTimeout(TimeSpan activityTimeout, TimeSpan shortInactivityTimeout)
        {
            foreach (var intervalStream in Intervals.Values)
            {
                Interval previousInterval = null;
                for (var i = 0; i < intervalStream.Count; i++)
                {
                    var interval = intervalStream[i];
                    if (previousInterval != null)
                    {
                        var gapDuration = interval.Start - previousInterval.End;
                        if (gapDuration <= activityTimeout)
                        {
                            previousInterval.End = interval.Start;
                        }
                        else
                        {
                            previousInterval.End += activityTimeout;
                            interval = CreateInactivity(previousInterval.End, interval.Start, shortInactivityTimeout);
                            intervalStream.Insert(i, interval);
                        }
                    }

                    previousInterval = interval;
                }

                if (previousInterval != null)
                {
                    previousInterval.End += activityTimeout;
                }
            }
        }

        private static Interval CreateInactivity(DateTime start, DateTime end, TimeSpan shortInactivityTimeout)
        {
            var gapDuration = end - start;
            return new Interval
            {
                Start = start,
                Activity = gapDuration <= shortInactivityTimeout ? Activity.Inactive : Activity.InactiveLong,
                End = end
            };
        }
    }
}
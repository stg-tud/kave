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
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Csv;
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
        private ActivityEvent _lastEvent;

        public ActivityIntervalProcessor()
        {
            Intervals = new Dictionary<Developer, IList<Interval>>();
            RegisterFor<ActivityEvent>(Handle);
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
            else
            {
                HandleWithInterval(@event);
            }
        }

        private void HandleWithInterval(ActivityEvent @event)
        {
            if (EndsAwayButNotInAwayInterval(@event))
            {
                StartInterval(_currentInterval.End, Activity.Away, @event.GetTriggeredAt());
            }

            if (InConcurrentOtherInterval(@event))
            {
                _currentInterval.Activity = GetIntervalActivity(@event);
                _currentInterval.End = GetEnd(@event);
            }

            if (InConcurrentWaitingInterval(@event))
            {
                var waitingEnd = _currentInterval.End;
                _currentInterval.End = @event.GetTriggeredAt();
                StartInterval(@event);
                StartInterval(_currentInterval.End, Activity.Waiting, waitingEnd);
            }
            else if (RequiresNewInterval(@event))
            {
                Asserts.That(
                    @event.GetTriggeredAt() >= _currentInterval.End,
                    "concurrent activities: \n{0}\n{1}",
                    _lastEvent,
                    @event);

                var previousInterval = _currentInterval;

                StartInterval(@event);

                if (previousInterval.Activity == Activity.Away)
                {
                    previousInterval.End = _currentInterval.Start;
                }
            }
            else
            {
                var newEnd = GetEnd(@event);
                if (newEnd > _currentInterval.End)
                {
                    _currentInterval.End = newEnd;
                }
            }
            _lastEvent = @event;
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

        private Activity GetIntervalActivity(ActivityEvent @event)
        {
            switch (@event.Activity)
            {
                case Activity.LeaveIDE:
                    return Activity.Away;
                case Activity.EnterIDE:
                    return Activity.Other;
                case Activity.Any:
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

        private bool InConcurrentOtherInterval(ActivityEvent @event)
        {
            return _currentInterval.Activity == Activity.Other && _currentInterval.Start.Equals(@event.GetTriggeredAt());
        }

        private bool InConcurrentWaitingInterval(ActivityEvent @event)
        {
            return _currentInterval.Activity == Activity.Waiting && @event.Activity != Activity.Waiting &&
                   _currentInterval.End > @event.GetTriggeredAt();
        }

        private bool RequiresNewInterval(ActivityEvent @event)
        {
            return (_currentInterval.Activity != GetIntervalActivity(@event) && @event.Activity != Activity.Any) ||
                   @event.GetTriggeredAt() > _currentInterval.End;
        }

        public IDictionary<Developer, IList<Interval>> GetIntervalsWithCorrectTimeouts(TimeSpan activityTimeout,
            TimeSpan shortInactivityTimeout)
        {
            IDictionary<Developer, IList<Interval>> correctedIntervals = CloneIntervals();
            foreach (var intervalStream in correctedIntervals.Values)
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
                            if (interval.Activity == previousInterval.Activity)
                            {
                                previousInterval.End = interval.End;
                                intervalStream.RemoveAt(i);
                                i--;
                                interval = previousInterval;
                            }
                            else
                            {
                                previousInterval.End = interval.Start;
                            }
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
            return correctedIntervals;
        }

        private Dictionary<Developer, IList<Interval>> CloneIntervals()
        {
            var intervals = new Dictionary<Developer, IList<Interval>>();
            foreach (var developerStream in Intervals)
            {
                intervals[developerStream.Key] = new List<Interval>();
                foreach (var interval in developerStream.Value)
                {
                    intervals[developerStream.Key].Add(
                        new Interval
                        {
                            Start = interval.Start,
                            Activity = interval.Activity,
                            End = interval.End
                        });
                }
            }
            return intervals;
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

        public string IntervalsToDeveloperBudgetCsv(TimeSpan activityTimeout, TimeSpan shortInactivityTimeout)
        {
            var builder = new CsvBuilder();
            foreach (var developerStream in GetIntervalsWithCorrectTimeouts(activityTimeout, shortInactivityTimeout))
            {
                var days = 0;
                var numberOfShortInactivities = 0;
                var numberOfLongInactivities = 0;
                Interval previousInterval = null;
                IDictionary<Activity, TimeSpan> budget = new Dictionary<Activity, TimeSpan>();
                foreach (var interval in developerStream.Value)
                {
                    if (interval.Start.Date != interval.End.Date ||
                        (previousInterval != null && previousInterval.End.Date != interval.Start.Date))
                    {
                        days++;
                    }
                    if (interval.Activity == Activity.Inactive)
                    {
                        numberOfShortInactivities++;
                    }
                    if (interval.Activity == Activity.InactiveLong)
                    {
                        numberOfLongInactivities++;
                    }
                    budget[interval.Activity] += interval.End - interval.Start;
                    previousInterval = interval;
                }

                builder["developer"] = developerStream.Key.Id;
                builder["active days"] = days;
                builder["# of intervals"] = developerStream.Value.Count;
                builder["# of short inactivities"] = numberOfShortInactivities;
                builder["# of long inactivities"] = numberOfLongInactivities;
                foreach (var activity in budget)
                {
                    builder[activity.Key.ToString()] = activity.Value.TotalMilliseconds/days;
                }
            }
            return builder.Build();
        }
    }
}
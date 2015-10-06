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
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Activities.Intervals
{
    internal class ActivityIntervalProcessor : IntervalProcessor<Activity>
    {
        protected override void HandleWithCurrentInterval(ActivityEvent @event)
        {
            if (EndsAwayButNotInAwayInterval(@event))
            {
                StartInterval(CurrentInterval.End, Activity.Away, @event.GetTriggeredAt());
            }

            if (InConcurrentOtherInterval(@event))
            {
                CurrentInterval.Id = GetIntervalId(@event);
                CurrentInterval.End = GetEnd(@event);
            }

            if (InConcurrentWaitingInterval(@event))
            {
                var waitingEnd = CurrentInterval.End;
                CurrentInterval.End = @event.GetTriggeredAt();
                StartInterval(@event);
                StartInterval(CurrentInterval.End, Activity.Waiting, waitingEnd);
            }
            else if (RequiresNewInterval(@event))
            {
                var previousInterval = CurrentInterval;

                StartInterval(@event);

                if (previousInterval.Id == Activity.Away || previousInterval.End > CurrentInterval.Start)
                {
                    var diff = (previousInterval.End - CurrentInterval.Start).TotalMilliseconds;
                    if (diff > 1000)
                    {
                        Console.WriteLine(@"WARNING: Ignoring {0}ms of event duration.", diff);
                    }
                    previousInterval.End = CurrentInterval.Start;
                }
            }
            else
            {
                var newEnd = GetEnd(@event);
                if (newEnd > CurrentInterval.End)
                {
                    CurrentInterval.End = newEnd;
                }
            }
        }

        protected override Activity GetIntervalId(ActivityEvent @event)
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

        private bool EndsAwayButNotInAwayInterval(ActivityEvent @event)
        {
            return @event.Activity == Activity.EnterIDE && CurrentInterval.Id != Activity.Away;
        }

        private bool InConcurrentOtherInterval(ActivityEvent @event)
        {
            return CurrentInterval.Id == Activity.Other && CurrentInterval.Start.Equals(@event.GetTriggeredAt());
        }

        private bool InConcurrentWaitingInterval(ActivityEvent @event)
        {
            return CurrentInterval.Id == Activity.Waiting && @event.Activity != Activity.Waiting &&
                   CurrentInterval.End > @event.GetTriggeredAt();
        }

        private bool RequiresNewInterval(ActivityEvent @event)
        {
            return (CurrentInterval.Id != GetIntervalId(@event) && @event.Activity != Activity.Any) ||
                   @event.GetTriggeredAt() > CurrentInterval.End;
        }

        public IDictionary<Developer, IList<Interval<Activity>>> GetIntervalsWithCorrectTimeouts(
            TimeSpan activityTimeout,
            TimeSpan shortInactivityTimeout)
        {
            IDictionary<Developer, IList<Interval<Activity>>> correctedIntervals = CloneIntervals();
            foreach (var intervalStream in correctedIntervals.Values)
            {
                Interval<Activity> previousInterval = null;
                for (var i = 0; i < intervalStream.Count; i++)
                {
                    var interval = intervalStream[i];
                    if (previousInterval != null)
                    {
                        var gapDuration = interval.Start - previousInterval.End;
                        if (gapDuration <= activityTimeout)
                        {
                            if (interval.Id == previousInterval.Id)
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

        private Dictionary<Developer, IList<Interval<Activity>>> CloneIntervals()
        {
            var intervals = new Dictionary<Developer, IList<Interval<Activity>>>();
            foreach (var developerStream in Intervals)
            {
                intervals[developerStream.Key] = new List<Interval<Activity>>();
                foreach (var interval in developerStream.Value)
                {
                    intervals[developerStream.Key].Add(
                        new Interval<Activity>
                        {
                            Start = interval.Start,
                            Id = interval.Id,
                            End = interval.End
                        });
                }
            }
            return intervals;
        }

        private static Interval<Activity> CreateInactivity(DateTime start, DateTime end, TimeSpan shortInactivityTimeout)
        {
            var gapDuration = end - start;
            return new Interval<Activity>
            {
                Start = start,
                Id = gapDuration <= shortInactivityTimeout ? Activity.Inactive : Activity.InactiveLong,
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
                Interval<Activity> previousInterval = null;
                IDictionary<Activity, TimeSpan> budget = new Dictionary<Activity, TimeSpan>();
                foreach (var interval in developerStream.Value)
                {
                    if (interval.Start.Date != interval.End.Date ||
                        (previousInterval != null && previousInterval.End.Date != interval.Start.Date))
                    {
                        days++;
                    }
                    if (interval.Id == Activity.Inactive)
                    {
                        numberOfShortInactivities++;
                    }
                    if (interval.Id == Activity.InactiveLong)
                    {
                        numberOfLongInactivities++;
                    }

                    if (!budget.ContainsKey(interval.Id))
                    {
                        budget[interval.Id] = TimeSpan.Zero;
                    }
                    budget[interval.Id] += interval.Duration;
                    previousInterval = interval;
                }

                builder.StartRow();
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
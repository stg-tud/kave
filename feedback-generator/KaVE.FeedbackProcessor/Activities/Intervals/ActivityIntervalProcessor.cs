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
using KaVE.Commons.Utils.Csv;
using KaVE.Commons.Utils.DateTime;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Activities.Intervals
{
    internal class ActivityIntervalProcessor : IntervalProcessor<Activity>
    {
        public class LostTime
        {
            public int Frequency;
            public TimeSpan Time;
        }

        public IDictionary<Activity, LostTime> LostTimeStatistics = new Dictionary<Activity, LostTime>();
        public ISet<Pair<Activity>> OverlappingActivities = new HashSet<Pair<Activity>>(); 

        public ActivityIntervalProcessor()
        {
            foreach (var activity in Enum.GetValues(typeof(Activity)).Cast<Activity>())
            {
                LostTimeStatistics[activity] = new LostTime {Frequency = 0, Time = TimeSpan.Zero};
            }
        }

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
                if (CurrentInterval.End < waitingEnd)
                {
                    StartInterval(CurrentInterval.End, Activity.Waiting, waitingEnd);
                }
            }
            else if (RequiresNewInterval(@event))
            {
                var previousInterval = CurrentInterval;

                StartInterval(@event);

                if (previousInterval.Id == Activity.Away || previousInterval.End > CurrentInterval.Start)
                {
                    var diff = previousInterval.End - CurrentInterval.Start;
                    OverlappingActivities.Add(new Pair<Activity>(previousInterval.Id, CurrentInterval.Id));
                    LostTimeStatistics[CurrentInterval.Id].Frequency++;
                    LostTimeStatistics[CurrentInterval.Id].Time += diff;
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
                var days = new HashSet<DateTime>();
                var numberOfShortInactivities = 0;
                var numberOfLongInactivities = 0;
                IDictionary<Activity, TimeSpan> budget = new Dictionary<Activity, TimeSpan>();
                foreach (var interval in developerStream.Value)
                {
                    days.Add(interval.Start.Date);

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
                }

                builder.StartRow();
                builder["developer"] = developerStream.Key.Id;
                builder["active days"] = days.Count;
                builder["# of intervals"] = developerStream.Value.Count;
                builder["# of short inactivities"] = numberOfShortInactivities;
                builder["# of long inactivities"] = numberOfLongInactivities;
                foreach (var activity in budget)
                {
                    builder[activity.Key.ToString()] = (int) Math.Round(activity.Value.TotalSeconds);
                }
            }
            return builder.Build();
        }

        public string InactivityStatisticToCsv(TimeSpan activityTimeout, params TimeSpan[] shortInactivityTimeouts)
        {
            var builder = new CsvBuilder();
            foreach (var shortInactivityTimeout in shortInactivityTimeouts)
            {
                var inactivity = TimeSpan.Zero;
                var numberOfInactivityPeriods = 0;
                var longInactivity = TimeSpan.Zero;
                var numberOfLongInactivityPeriods = 0;
                var numberOfActivitySprees = 0;
                var days = new HashSet<string>();
                foreach (var intervalStream in GetIntervalsWithCorrectTimeouts(activityTimeout, shortInactivityTimeout))
                {
                    var lastWasInactivity = false;
                    foreach (var interval in intervalStream.Value)
                    {
                        days.Add(string.Format("{0}-{1}", intervalStream.Key.Id, interval.Start.Date));

                        if (interval.Id == Activity.Inactive)
                        {
                            numberOfInactivityPeriods++;
                            inactivity += interval.Duration;
                            lastWasInactivity = true;
                        }
                        else if (interval.Id == Activity.InactiveLong)
                        {
                            numberOfLongInactivityPeriods++;
                            longInactivity += interval.Duration;
                            lastWasInactivity = true;
                        }
                        else if (lastWasInactivity)
                        {
                            numberOfActivitySprees++;
                        }
                    }
                }
                builder.StartRow();
                builder["Threshold (s)"] = shortInactivityTimeout.RoundedTotalSeconds();
                builder["Inactivity (s)"] = inactivity.RoundedTotalSeconds();
                builder["# of Inactivities"] = numberOfInactivityPeriods;
                builder["Long Inactivity (s)"] = longInactivity.RoundedTotalSeconds();
                builder["# of Long Inactivities"] = numberOfLongInactivityPeriods;
                builder["# of activity sprees"] = numberOfActivitySprees;
                builder["Days"] = days.Count;
            }
            return builder.Build();
        }
    }
}
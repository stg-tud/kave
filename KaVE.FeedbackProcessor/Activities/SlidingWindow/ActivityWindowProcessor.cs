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
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    internal class ActivityWindowProcessor : BaseEventProcessor
    {
        private readonly IActivityMergeStrategy _strategy;
        private readonly TimeSpan _windowSpan;

        private readonly Queue<ActivityEvent> _queue = new Queue<ActivityEvent>();

        private Window _currentWindow;
        private Developer _currentDeveloper;

        public ActivityWindowProcessor(IActivityMergeStrategy strategy, TimeSpan windowSpan)
        {
            _strategy = strategy;
            _windowSpan = windowSpan;
            ActivityStreams = new Dictionary<Developer, IDictionary<DateTime, ActivityStream>>();
            RegisterFor<ActivityEvent>(ProcessActivities);
        }

        public IDictionary<Developer, IDictionary<DateTime, ActivityStream>> ActivityStreams { get; private set; }

        public override void OnStreamStarts(Developer developer)
        {
            _currentDeveloper = developer;
            ActivityStreams[developer] = new Dictionary<DateTime, ActivityStream>();
            _currentWindow = null;
        }

        private void ProcessActivities(ActivityEvent @event)
        {
            EnsureInitialized(@event);

            while (FirstQueuedEventStartsBefore(@event))
            {
                ProcessActivities(_queue.Dequeue());
            }

            while (_currentWindow.EndsBeforeStartOf(@event))
            {
                ProceedToNextWindow(@event);
            }

            if (_currentWindow.EndsBeforeEndOf(@event))
            {
                var headAndTail = @event.SplitAt(_currentWindow.End);
                _currentWindow.Add(headAndTail.Item1);
                _queue.Enqueue(headAndTail.Item2);
            }
            else
            {
                _currentWindow.Add(@event);
            }
        }

        private void EnsureInitialized(ActivityEvent @event)
        {
            if (_currentWindow == null)
            {
                _currentWindow = CreateWindowStartingAt(@event);
            }
        }

        private bool FirstQueuedEventStartsBefore(ActivityEvent @event)
        {
            return _queue.Any() && _queue.Peek().GetTriggeredAt() < @event.GetTriggeredAt();
        }

        private void ProceedToNextWindow(ActivityEvent @event)
        {
            if (_currentWindow.IsNotEmpty || IsEmptyWindowRequired(@event))
            {
                AppendMergedWindowToStream();
            }

            if (_currentWindow.IsOnSameDayAs(@event))
            {
                _currentWindow = CreateFollowingWindow();
            }
            else
            {
                _strategy.Reset();
                _currentWindow = CreateWindowStartingAt(@event);
            }
        }

        private bool IsEmptyWindowRequired(ActivityEvent @event)
        {
            return _currentWindow.IsOnSameDayAs(@event);
        }

        private void AppendMergedWindowToStream()
        {
            if (!ActivityStreams[_currentDeveloper].ContainsKey(_currentWindow.Start.Date))
            {
                ActivityStreams[_currentDeveloper][_currentWindow.Start.Date] = new ActivityStream(_windowSpan);
            }
            ActivityStreams[_currentDeveloper][_currentWindow.Start.Date].Add(
                _strategy.Merge(_currentWindow));
        }

        private Window CreateWindowStartingAt(ActivityEvent @event)
        {
            return new Window(@event.GetTriggeredAt(), _windowSpan);
        }

        private Window CreateFollowingWindow()
        {
            return new Window(_currentWindow.End, _windowSpan);
        }

        public override void OnStreamEnds()
        {
            while (_queue.Any())
            {
                ProcessActivities(_queue.Dequeue());
            }

            if (_currentWindow != null)
            {
                AppendMergedWindowToStream();
            }
            _strategy.Reset();
        }

        public string ActivityStreamsToCsv(TimeSpan shortInactivityLimit, TimeSpan longInactivityThreshold)
        {
            var builder = new CsvBuilder();
            foreach (var developerWithStreams in ActivityStreams)
            {
                builder.StartRow();
                builder["Developer"] = developerWithStreams.Key.Id.ToString();
                builder["ActiveDays"] = developerWithStreams.Value.Count;
                var numberOfInactivities = 0;
                var inactivityTime = TimeSpan.Zero;
                var numberOfActivitySprees = 0;
                foreach (var dayWithStream in developerWithStreams.Value)
                {
                    var statistics = dayWithStream.Value.Evaluate(shortInactivityLimit, longInactivityThreshold);
                    foreach (var activityWithDuration in statistics)
                    {
                        builder[dayWithStream.Key.ToString("yyyy-MM-dd") + " " + activityWithDuration.Key] =
                            activityWithDuration.Value.TotalSeconds;
                    }
                    numberOfInactivities += statistics.NumberOfInactivityPeriods;
                    if (statistics.NumberOfInactivityPeriods > 0)
                    {
                        inactivityTime += statistics[Activity.Inactive];
                    }
                    numberOfActivitySprees += statistics.NumberOfInactivityPeriods +
                                              statistics.NumberOfLongInactivityPeriods + 1;
                }
                builder["# of Inactivities"] = numberOfInactivities;
                builder["inactivity duration"] = inactivityTime.TotalSeconds;
                builder["# of activity sprees"] = numberOfActivitySprees;
            }
            builder.StartRow();
            foreach (var field in builder.Fields.Skip(2))
            {
                builder[field] = field.Split(' ')[0];
            }
            builder.StartRow();
            foreach (var field in builder.Fields.Skip(2))
            {
                builder[field] = field.Split(' ')[1];
            }
            return builder.Build(CsvBuilder.SortFields.ByNameLeaveFirst);
        }

        public string DeveloperDayStatisticToCsv(TimeSpan shortInactivityLimit, TimeSpan longInactivityThreshold)
        {
            var builder = new CsvBuilder();

            foreach (var developerWithStreams in ActivityStreams)
            {
                foreach (var dayWithStream in developerWithStreams.Value)
                {
                    builder.StartRow();
                    var statistic = dayWithStream.Value.Evaluate(shortInactivityLimit, longInactivityThreshold);
                    builder["DevDay"] = dayWithStream.Key.ToString("yyyy-MM-dd") + "_" + developerWithStreams.Key.Id;
                    foreach (var activityWithDuration in statistic)
                    {
                        builder[activityWithDuration.Key.ToString()] = activityWithDuration.Value.TotalSeconds;
                    }
                    builder["Inactivities"] = statistic.NumberOfInactivityPeriods;
                    builder["LongInactivities"] = statistic.NumberOfLongInactivityPeriods;
                }
            }

            return builder.Build();
        }

        public string InactivityStatisticToCsv(TimeSpan shortInactivityLimit, params TimeSpan[] longInactivityThresholds)
        {
            var builder = new CsvBuilder();
            foreach (var longInactivityThreshold in longInactivityThresholds)
            {
                var inactivity = TimeSpan.Zero;
                var numberOfInactivityPeriods = 0;
                var longInactivity = TimeSpan.Zero;
                var numberOfLongInactivityPeriods = 0;
                var days = 0;
                foreach (var developerWithStreams in ActivityStreams)
                {
                    foreach (var dayWithStream in developerWithStreams.Value)
                    {
                        days++;
                        var statistic = dayWithStream.Value.Evaluate(shortInactivityLimit, longInactivityThreshold);
                        numberOfInactivityPeriods += statistic.NumberOfInactivityPeriods;
                        if (statistic.NumberOfInactivityPeriods > 0)
                        {
                            inactivity += statistic[Activity.Inactive];
                        }
                        numberOfLongInactivityPeriods += statistic.NumberOfLongInactivityPeriods;
                        if (statistic.NumberOfLongInactivityPeriods > 0)
                        {
                            longInactivity += statistic[Activity.InactiveLong];
                        }
                    }
                }
                builder.StartRow();
                builder["Threshold (ms)"] = longInactivityThreshold.TotalSeconds;
                builder["Inactivity (ms)"] = inactivity.TotalSeconds;
                builder["# of Inactivities"] = numberOfInactivityPeriods;
                builder["Long Inactivity (ms)"] = longInactivity.TotalSeconds;
                builder["# of Long Inactivities"] = numberOfLongInactivityPeriods;
                builder["Days"] = days;
            }
            return builder.Build();
        }
    }

    internal static class ActivityEventEx
    {
        internal static Pair<ActivityEvent> SplitAt(this ActivityEvent activityEvent, DateTime at)
        {
            var head = activityEvent.Clone();
            head.TerminatedAt = at;
            var tail = activityEvent.Clone();
            tail.TriggeredAt = head.TerminatedAt;
            tail.Duration = activityEvent.Duration - head.Duration;
            return new Pair<ActivityEvent>(head, tail);
        }

        private static ActivityEvent Clone(this ActivityEvent originalEvent)
        {
            var clone = new ActivityEvent
            {
                Activity = originalEvent.Activity
            };
            clone.CopyIDEEventPropertiesFrom(originalEvent);
            return clone;
        }
    }
}
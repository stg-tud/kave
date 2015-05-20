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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    internal class ActivityWindowProcessor : BaseEventProcessor
    {
        private readonly IActivityMergeStrategy _strategy;
        private readonly TimeSpan _windowSpan;

        private class Window
        {
            private readonly TimeSpan _span;
            private readonly IList<ActivityEvent> _events = new List<ActivityEvent>();

            public Window(DateTime start, TimeSpan span)
            {
                _span = span;
                Start = start;
            }

            public DateTime Start { get; set; }

            public DateTime End
            {
                get { return Start + _span; }
            }

            public void Add(ActivityEvent activityEvent)
            {
                _events.Add(activityEvent);
            }

            public bool IsNotEmpty
            {
                get { return _events.Count > 0; }
            }

            public bool EndsBefore(IDEEvent @event)
            {
                return End <= @event.TriggeredAt;
            }

            public bool IsOnSameDayAs(IDEEvent @event)
            {
                return End.Date == @event.GetTriggeredAt().Date;
            }

            public IList<Activity> GetActivities()
            {
                return _events.Select(e => e.Activity).ToList();
            }
        }

        private Window _currentWindow;
        private Developer _currentDeveloper;

        public interface IActivityMergeStrategy
        {
            Activity Merge(IList<Activity> window);
        }

        public ActivityWindowProcessor(IActivityMergeStrategy strategy, TimeSpan windowSpan)
        {
            _strategy = strategy;
            _windowSpan = windowSpan;
            ActivityStream = new Dictionary<Developer, IDictionary<DateTime, ActivityStream>>();
            RegisterFor<ActivityEvent>(ProcessActivities);
        }

        public IDictionary<Developer, IDictionary<DateTime, ActivityStream>> ActivityStream { get; private set; }

        public override void OnStreamStarts(Developer developer)
        {
            _currentDeveloper = developer;
            ActivityStream[developer] = new Dictionary<DateTime, ActivityStream>();
            _currentWindow = null;
        }

        private void ProcessActivities(ActivityEvent @event)
        {
            if (_currentWindow == null)
            {
                _currentWindow = CreateWindowStartingAt(@event.GetTriggeredAt());
            }

            while (_currentWindow.EndsBefore(@event))
            {
                if (_currentWindow.IsOnSameDayAs(@event) || _currentWindow.IsNotEmpty)
                {
                    AppendMergedWindowToStream();
                }

                if (_currentWindow.IsOnSameDayAs(@event))
                {
                    _currentWindow = CreateWindowStartingAt(_currentWindow.End);
                }
                else
                {
                    _currentWindow = CreateWindowStartingAt(@event.GetTriggeredAt());
                }
            }

            _currentWindow.Add(@event);
        }

        private void AppendMergedWindowToStream()
        {
            if (!ActivityStream[_currentDeveloper].ContainsKey(_currentWindow.Start.Date))
            {
                ActivityStream[_currentDeveloper][_currentWindow.Start.Date] = new ActivityStream(_windowSpan);
            }
            ActivityStream[_currentDeveloper][_currentWindow.Start.Date].Add(_strategy.Merge(_currentWindow.GetActivities()));
        }

        private Window CreateWindowStartingAt(DateTime windowStart)
        {
            return new Window(windowStart, _windowSpan);
        }

        public override void OnStreamEnds()
        {
            AppendMergedWindowToStream();
        }
    }
}
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
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    internal class ActivityWindowProcessor : BaseEventProcessor
    {
        private readonly IActivityMergeStrategy _strategy;
        private readonly TimeSpan _windowSpan;

        private IList<ActivityEvent> _window;

        private DateTime WindowStart { get; set; }

        private DateTime WindowEnd
        {
            get { return WindowStart + _windowSpan; }
        }

        public interface IActivityMergeStrategy
        {
            Activity Merge(IList<Activity> window);
        }

        public ActivityWindowProcessor(IActivityMergeStrategy strategy, TimeSpan windowSpan)
        {
            _strategy = strategy;
            _windowSpan = windowSpan;
            RegisterFor<ActivityEvent>(ProcessActivities);
        }

        private void ProcessActivities(ActivityEvent @event)
        {
            var triggeredAt = @event.GetTriggeredAt();

            if (_window == null)
            {
                StartWindowAt(triggeredAt);
            }

            while (WindowEnd <= @event.TriggeredAt && !(_window.Count == 0 && WindowEnd.Date != triggeredAt.Date))
            {
                EndWindowAndShiftWindow();
            }

            _window.Add(@event);
        }

        private void EndWindowAndShiftWindow()
        {
            _strategy.Merge(_window.Select(e => e.Activity).ToList());

            StartWindowAt(WindowEnd);
        }

        private void StartWindowAt(DateTime windowStart)
        {
            _window = new List<ActivityEvent>();
            WindowStart = windowStart;
        }

        public override void OnStreamEnds()
        {
            EndWindowAndShiftWindow();
        }
    }
}

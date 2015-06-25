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

using System.Collections.Generic;
using System.Linq;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    internal abstract class WeightedActivityMergeStrategy : IActivityMergeStrategy
    {
        private static readonly Activity[] SpecialActivities = {Activity.Any, Activity.LeaveIDE, Activity.EnterIDE};

        private Activity? _lastActivity;
        private bool _leftIDE;

        public Activity Merge(Window window)
        {
            if (window.IsNotEmpty)
            {
                _lastActivity = MergeNonEmptyWindow(window);
                _leftIDE = ContainsLeaveIDEActivity(window);
            }
            else
            {
                _lastActivity = EmptyWindowReplacement();
            }
            return _lastActivity.Value;
        }

        private static bool ContainsLeaveIDEActivity(Window window)
        {
            return window.Events.Any(e => e.Activity == Activity.LeaveIDE);
        }

        private Activity EmptyWindowReplacement()
        {
            return _leftIDE ? Activity.Away : Activity.Inactive;
        }

        private Activity MergeNonEmptyWindow(Window window)
        {
            var windowWithoutAny = WithoutSpecialActivities(window);
            if (windowWithoutAny.IsNotEmpty)
            {
                return SelectsRepresentativeActivity(windowWithoutAny);
            }
            if (_lastActivity == null || IsInactivity(_lastActivity))
            {
                return Activity.Other;
            }
            return _lastActivity.Value;
        }

        private Activity SelectsRepresentativeActivity(Window window)
        {
            var representativeActivity = Activity.Any;
            var maxActivityWeight = 0;
            foreach (var activity in window.Events.Select(e => e.Activity))
            {
                var weightOfActivity = GetWeightOfActivity(window, activity);
                if (weightOfActivity >= maxActivityWeight)
                {
                    representativeActivity = activity;
                    maxActivityWeight = weightOfActivity;
                }
            }
            return representativeActivity;
        }

        protected abstract int GetWeightOfActivity(Window window, Activity activity);

        private static Window WithoutSpecialActivities(Window window)
        {
            var newWindow = new Window(window.Start, window.Span);
            foreach (var activity in window.Events.Where(e => !SpecialActivities.Contains(e.Activity)))
            {
                newWindow.Add(activity);
            }
            return newWindow;
        }

        private static bool IsInactivity(Activity? lastActivity)
        {
            return lastActivity == Activity.Away || lastActivity == Activity.Inactive;
        }

        public void Reset()
        {
            _lastActivity = null;
            _leftIDE = false;
        }
    }
}
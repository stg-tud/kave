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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    internal class FrequencyActivityMergeStrategy : IActivityMergeStrategy
    {
        private Activity? _lastActivity;

        public Activity Merge(IList<Activity> window, IList<ActivityEvent> window2)
        {
            if (IsEmptyWindow(window))
            {
                _lastActivity = Activity.Waiting;
            }
            else
            {
                _lastActivity = MergeNonEmptyWindow(window);
            }
            return _lastActivity.Value;
        }

        private static bool IsEmptyWindow(ICollection<Activity> window)
        {
            return window.Count == 0;
        }

        private Activity MergeNonEmptyWindow(IEnumerable<Activity> window)
        {
            var windowWithoutAny = WithoutAnyActivity(window);
            if (!IsEmptyWindow(windowWithoutAny))
            {
                return GetLastMostFrequentActivity(windowWithoutAny);
            }
            if (_lastActivity == null || IsInactivity(_lastActivity))
            {
                return Activity.Other;
            }
            return _lastActivity.Value;
        }

        private static Activity GetLastMostFrequentActivity(IList<Activity> windowWithoutAny)
        {
            var activities = new Multiset<Activity>(windowWithoutAny);
            var maxFrequency = activities.EntryDictionary.Max(kvp => kvp.Value);
            var mostFrequentActivities =
                activities.EntryDictionary.Where(kvp => kvp.Value == maxFrequency).Select(kvp => kvp.Key);
            return windowWithoutAny.Last(mostFrequentActivities.Contains);
        }

        private static IList<Activity> WithoutAnyActivity(IEnumerable<Activity> window)
        {
            return window.Where(a => a != Activity.Any).ToList();
        }

        private static bool IsInactivity(Activity? lastActivity)
        {
            return lastActivity == Activity.Away || lastActivity == Activity.Waiting;
        }

        public void Reset()
        {
            _lastActivity = null;
        }
    }
}
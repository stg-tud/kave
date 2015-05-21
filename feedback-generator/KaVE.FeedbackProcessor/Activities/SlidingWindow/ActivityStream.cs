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
using System.Collections;
using System.Collections.Generic;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    public class ActivityStream : IEnumerable<Activity>
    {
        private readonly TimeSpan _windowSpan;
        private readonly IList<Activity> _activities;

        public ActivityStream(TimeSpan windowSpan)
        {
            _windowSpan = windowSpan;
            _activities = new List<Activity>();
        }

        public void Add(Activity activity)
        {
            _activities.Add(activity);
        }

        public void AddAll(IEnumerable<Activity> activities)
        {
            foreach (var activity in activities)
            {
                Add(activity);
            }
        }

        public IDictionary<Activity, TimeSpan> Evaluate(TimeSpan awayThreshold)
        {
            var statistic = new Dictionary<Activity, TimeSpan>();
            var waitingDuration = TimeSpan.Zero;
            foreach (var activity in _activities)
            {
                if (activity == Activity.Waiting)
                {
                    waitingDuration += _windowSpan;
                }
                else if (waitingDuration > TimeSpan.Zero)
                {
                    if (WasAway(activity, waitingDuration, awayThreshold))
                    {
                        Add(statistic, Activity.Waiting, -waitingDuration);
                        Add(statistic, Activity.Away, waitingDuration);
                    }
                    waitingDuration = TimeSpan.Zero;
                }

                if (IsLeaveOrEnter(activity))
                {
                    Add(statistic, Activity.Away, _windowSpan);
                }
                else
                {
                    Add(statistic, activity, _windowSpan);
                }

            }
            return statistic;
        }

        private static bool WasAway(Activity activity, TimeSpan waitingDuration, TimeSpan awayThreshold)
        {
            return waitingDuration > awayThreshold || activity == Activity.EnterIDE;
        }

        private static bool IsLeaveOrEnter(Activity activity)
        {
            return activity == Activity.LeaveIDE || activity == Activity.EnterIDE;
        }

        private static void Add(IDictionary<Activity, TimeSpan> statistic, Activity activity, TimeSpan windowSpan)
        {
            if (statistic.ContainsKey(activity))
            {
                statistic[activity] += windowSpan;
            }
            else
            {
                statistic[activity] = windowSpan;
            }
        }

        public IEnumerator<Activity> GetEnumerator()
        {
            return _activities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
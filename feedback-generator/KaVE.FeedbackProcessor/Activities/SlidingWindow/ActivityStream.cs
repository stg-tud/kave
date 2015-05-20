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
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    public class ActivityStream
    {
        private readonly TimeSpan _windowSpan;

        public ActivityStream(IList<Activity> activities, TimeSpan windowSpan)
        {
            _windowSpan = windowSpan;
            Activities = activities;
        }

        public IList<Activity> Activities { get; private set; }

        public IDictionary<Activity, TimeSpan> Evaluate(TimeSpan awayThreshold)
        {
            var statistic = new Dictionary<Activity, TimeSpan>();
            var waitingDuration = TimeSpan.Zero;
            foreach (var activity in Activities)
            {
                if (activity == Activity.Waiting)
                {
                    waitingDuration += _windowSpan;
                }
                else if (waitingDuration > TimeSpan.Zero)
                {
                    if (waitingDuration > awayThreshold)
                    {
                        Add(statistic, Activity.Waiting, -waitingDuration);
                        Add(statistic, Activity.Away, waitingDuration);
                    }
                    waitingDuration = TimeSpan.Zero;
                }
                Add(statistic, activity, _windowSpan);
            }
            return statistic;
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
    }
}
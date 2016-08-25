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
using System.Linq;
using KaVE.Commons.Utils.DateTimes;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    internal class DurationActivityMergingStrategy : WeightedActivityMergeStrategy
    {
        private static readonly TimeSpan MinimalActivityDuration = TimeSpan.FromMilliseconds(100);

        protected override int GetWeightOfActivity(Window window, Activity activity)
        {
            return window.Events.Where(e => e.Activity == activity).Sum(a => GetDuration(a, window));
        }

        private static int GetDuration(ActivityEvent a, Window window)
        {
            TimeSpan duration;
            var minimalDuration = GetMinimalDuration(window);
            if (a.Duration == null || a.Duration < minimalDuration)
            {
                duration = minimalDuration;
            }
            else
            {
                duration = a.Duration.Value;
            }
            return (int) duration.TotalMilliseconds;
        }

        private static TimeSpan GetMinimalDuration(Window window)
        {
            var relativeMinimum = window.Span.Times(1.0/window.Events.Count);
            return relativeMinimum < MinimalActivityDuration ? relativeMinimum : MinimalActivityDuration;
        }
    }
}
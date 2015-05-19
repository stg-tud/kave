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
    internal class FrequencyActivityMergeStrategy : ActivityWindowProcessor.IActivityMergeStrategy
    {
        public Activity Merge(IList<Activity> window)
        {
            if (window.Count == 0)
            {
                return Activity.Waiting;
            }

            var activities = new Multiset<Activity>(window);
            var maxFrequency = activities.EntryDictionary.Max(kvp => kvp.Value);
            var mostFrequentActivities = activities.EntryDictionary.Where(kvp => kvp.Value == maxFrequency).Select(kvp => kvp.Key);
            return window.Last(mostFrequentActivities.Contains);
        }
    }
}
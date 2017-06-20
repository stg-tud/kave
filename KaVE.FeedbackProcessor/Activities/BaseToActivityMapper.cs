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
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Utils;
using ActivityEvent = KaVE.FeedbackProcessor.Activities.Model.ActivityEvent;

namespace KaVE.FeedbackProcessor.Activities
{
    internal abstract class BaseToActivityMapper : BaseEventMapper
    {
        protected void InsertActivity(IDEEvent baseEvent, Activity activity)
        {
            InsertActivities(baseEvent, activity);
        }

        protected void InsertActivities(IDEEvent baseEvent, params Activity[] activities)
        {
            var events = new HashSet<IDEEvent>();
            foreach (var activity in activities)
            {
                events.Add(CreateActivityEvent(baseEvent, activity));
            }
            ReplaceCurrentEventWith(events.ToArray());
        }

        private static ActivityEvent CreateActivityEvent(IDEEvent baseEvent, Activity activity)
        {
            var activityEvent = new ActivityEvent
            {
                Activity = activity
            };
            activityEvent.CopyIDEEventPropertiesFrom(baseEvent);
            return activityEvent;
        }
    }
}
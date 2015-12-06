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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Model;
using ActivityEvent = KaVE.FeedbackProcessor.Activities.Model.ActivityEvent;

namespace KaVE.FeedbackProcessor.Activities.SlidingWindow
{
    internal class Window
    {
        public readonly TimeSpan Span;
        public readonly IList<ActivityEvent> Events = new KaVEList<ActivityEvent>();

        public Window(DateTime start, TimeSpan span)
        {
            Span = span;
            Start = start;
        }

        public DateTime Start { get; set; }

        public DateTime End
        {
            get { return Start + Span; }
        }

        public void Add(ActivityEvent activityEvent)
        {
            Events.Add(activityEvent);
        }

        public bool IsNotEmpty
        {
            get { return Events.Count > 0; }
        }

        public bool EndsBeforeStartOf(IDEEvent @event)
        {
            return End <= @event.TriggeredAt;
        }

        public bool EndsBeforeEndOf(IDEEvent @event)
        {
            return @event.TerminatedAt > End;
        }

        public bool IsOnSameDayAs(IDEEvent @event)
        {
            return End.Date == @event.GetTriggeredAt().Date;
        }

        public IList<Activity> GetActivities()
        {
            return Events.Select(e => e.Activity).ToList();
        }

        protected bool Equals(Window other)
        {
            return Span.Equals(other.Span) && Equals(Events, other.Events) && Start.Equals(other.Start);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Span.GetHashCode();
                hashCode = (hashCode*397) ^ (Events != null ? Events.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Start.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
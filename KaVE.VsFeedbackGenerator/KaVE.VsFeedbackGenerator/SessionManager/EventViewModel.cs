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
 *    - Dennis Albrecht
 */

using System;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class EventViewModel : ViewModelBase<EventViewModel>
    {
        public EventViewModel(IDEEvent evt)
        {
            Event = evt;
        }

        public IDEEvent Event { get; private set; }

        public string EventType
        {
            get
            {
                var eventTypeName = Event.GetType().Name;
                if (eventTypeName.EndsWith("Event"))
                {
                    eventTypeName = eventTypeName.Remove(eventTypeName.Length - 5);
                }
                return eventTypeName;
            }
        }

        public string Trigger
        {
            get { return Event.TriggeredBy.ToString(); }
        }

        public DateTime? StartDateTime
        {
            get { return Event.TriggeredAt; }
        }

        public double DurationInMilliseconds
        {
            get { return Event.Duration.HasValue ? Event.Duration.Value.TotalMilliseconds : 0; }
        }

        public string XamlContextRepresentation
        {
            get { return CompletionContext.ToXaml(); }
        }

        private Context CompletionContext
        {
            get
            {
                var completionEvent = Event as CompletionEvent;
                return completionEvent == null ? null : completionEvent.Context;
            }
        }

        public string XamlRawRepresentation
        {
            get { return Event.ToFormattedJson().AddJsonSyntaxHighlightingWithXaml(); }
        }

        public string Details
        {
            get { return Event.GetDetailsAsJson().AddJsonSyntaxHighlightingWithXaml(); }
        }

        protected bool Equals(EventViewModel other)
        {
            return string.Equals(Event, other.Event);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return Event.GetHashCode();
        }
    }
}
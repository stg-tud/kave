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
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class EventViewModel : ViewModelBase<EventViewModel>
    {
        private string _xamlContext;
        private string _xamlDetails;
        private string _xamlRaw;

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
            get { return _xamlContext ?? (_xamlContext = GetContextAsXaml()); }
        }

        private string GetContextAsXaml()
        {
            //var completionEvent = Event as CompletionEvent;
            //return completionEvent == null ? null : completionEvent.Context.ToXaml();
            return null;
        }

        public string Details
        {
            get
            {
                return _xamlDetails ?? (_xamlDetails = AddSyntaxHighlightingIfNotTooLong(Event.GetDetailsAsJson()));
            }
        }

        public string XamlRawRepresentation
        {
            get { return _xamlRaw ?? (_xamlRaw = AddSyntaxHighlightingIfNotTooLong(Event.ToFormattedJson())); }
        }

        /// <summary>
        /// Adds syntax highlighting (Xaml formatting) to the json, if the json ist not too long.
        /// This condition is because formatting very long strings takes ages.
        /// </summary>
        private static string AddSyntaxHighlightingIfNotTooLong(string json)
        {
            return json.Length > 50000 ? json : json.AddJsonSyntaxHighlightingWithXaml();
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
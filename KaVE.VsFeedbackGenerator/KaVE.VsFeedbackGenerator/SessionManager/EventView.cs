using System;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class EventView
    {
        public EventView(IDEEvent evt)
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

        public DateTime StartDateTime
        {
            get { return Event.TriggeredAt; }
        }

        public double DurationInMilliseconds
        {
            get { return Event.Duration.HasValue ? Event.Duration.Value.TotalMilliseconds : 0; }
        }

        public Context CompletionContext
        {
            get
            {
                var completionEvent = Event as CompletionEvent;
                return completionEvent == null ? null : completionEvent.Context;
            }
        }

        public string XamlContextRepresentation
        {
            get { return CompletionContext.ToXaml(); }
        }

        // TODO @Dennis: Der View aktualisiert sich nicht für ActionEvents (evlt. auch nocht andere), da bleibt bei mir einfach der Inhalt des zuletzt ausgewählten Events in der TextBox stehen...
        public string XamlRawRepresentation
        {
            get { return Event.ToFormattedJson().AddJsonSyntaxHighlightingWithXaml(); }
        }

        public string Details
        {
            get { return Event.GetDetailsAsJson().AddJsonSyntaxHighlightingWithXaml(); }
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils.Serialization;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class EventView
    {
        private static readonly PropertyInfo[] IDEEventProperties = typeof (IDEEvent).GetProperties();

        public EventView(IDEEvent evt)
        {
            Event = evt;
        }

        public IDEEvent Event { get; private set; }

        public string EventType
        {
            get { return Event.GetType().Name; }
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

        // TODO @Dennis: Warum handled die eine toXaml()-Method den null-case und die andere nicht?
        public string XamlContextRepresentation
        {
            get { return CompletionContext == null ? null : CompletionContext.ToXaml(); }
        }

        // TODO @Dennis: Der View aktualisiert sich nicht für ActionEvents (evlt. auch nocht andere), da bleibt bei mir einfach der Inhalt des zuletzt ausgewählten Events in der TextBox stehen...
        public string XamlRawRepresentation
        {
            get { return Event.ToXaml(); }
        }

        // TODO @Sven: Extract to dedicated converter
        public string Details
        {
            get
            {
                var details = Event.ToJson(new NameToIdentifierConverter(), new EnumToStringConverter());
                var detailLines = details.Split(
                    Environment.NewLine.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries);
                var specializedDetails = detailLines.Where(IsSpecializedEventInformation).Join(Environment.NewLine);
                // remove opening "{\r\n" and closing ",\r\n}"
                return specializedDetails.Substring(3, specializedDetails.Length - 7);
            }
        }

        /// <summary>
        /// A line is considered to contain specialized information, if it does not contain the name of the property of
        /// the <see cref="IDEEvent"/> type, like, for example, "  'IDESessionUUID': '0xDEADBEEF'" does.
        /// </summary>
        private static bool IsSpecializedEventInformation(string detailLine)
        {
            return IDEEventProperties.All(ideEventProperty => !detailLine.Contains(ideEventProperty.Name));
        }
    }
}
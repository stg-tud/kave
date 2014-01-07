using System;
using System.Text;
using KaVE.Model.Events;
using KaVE.Utils.Serialization;
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
                return Event.GetType().Name;
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                return Event.TriggeredAt;
            }
        }

        public string StartTime
        {
            get
            {
                return StartDateTime.ToString("HH:mm:ss");
            }
        }

        public string Content
        {
            get
            {
                var content = new StringBuilder();
                content.Append(Event.GetType().Name).Append(Environment.NewLine);
                content.Append(Event.ToJson(new NameToJsonConverter()).Replace("  ", "      "));
                return content.ToString();
            }
        }
    }
}

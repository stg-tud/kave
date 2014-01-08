using System;
using System.Linq;
using System.Text;
using JetBrains.Util;
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

        public string Trigger
        {
            get
            {
                return Event.TriggeredBy.ToString();
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                return Event.TriggeredAt;
            }
        }

        public double DurationInMilliseconds
        {
            get
            {
                return Event.Duration.HasValue ? Event.Duration.Value.TotalMilliseconds : 0;
            }
        }

        public string Content
        {
            get
            {
                // TODO test this logic!!!
                var details = Event.ToJson(new NameToJsonConverter()).Replace("  ", "      ");
                var detailLines = details.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var numberOfRelevantDetailLines = detailLines.Length - 10;
                var relevantDetailLines = new string[numberOfRelevantDetailLines];
                Array.Copy(detailLines, 1, relevantDetailLines, 0, numberOfRelevantDetailLines);
                return relevantDetailLines.Join(Environment.NewLine);
            }
        }
    }
}

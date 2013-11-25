using System;
using System.Text;
using KaVE.Model.Events;
using KaVE.Utils.Serialization;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class EventView
    {
        private readonly IDEEvent _evt;

        public EventView(IDEEvent evt)
        {
            _evt = evt;
        }

        public DateTime StartTime
        {
            get
            {
                return _evt.TriggeredAt;
            }
        }

        public string TriggeredBy
        {
            get
            {
                return _evt.TriggeredBy.ToString();
            }
        }

        public string Content
        {
            get
            {
                var content = new StringBuilder();
                content.Append(_evt.GetType().Name).Append(Environment.NewLine);
                content.Append(_evt.ToJson(new NameToJsonConverter()).Replace("  ", "      "));
                return content.ToString();
            }
        }
    }
}

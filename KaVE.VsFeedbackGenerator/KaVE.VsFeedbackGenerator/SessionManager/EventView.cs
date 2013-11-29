using System;
using System.Collections.Generic;
using System.Text;
using KaVE.Model.Events;
using KaVE.Utils.Serialization;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class EventView
    {
        private readonly IDEEvent _evt;

        public EventView(IDEEvent evt)
        {
            _evt = evt;
        }

        public string EventType
        {
            get
            {
                return _evt.GetType().Name;
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                return _evt.TriggeredAt;
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
                content.Append(_evt.GetType().Name).Append(Environment.NewLine);
                content.Append(_evt.ToJson(new NameToJsonConverter()).Replace("  ", "      "));
                return content.ToString();
            }
        }
    }
}

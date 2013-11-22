using System;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionEvent
    {
        private readonly IDEEvent _evt;

        public SessionEvent(IDEEvent evt)
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
                return JsonConvert.SerializeObject(_evt, Formatting.Indented, new NameToJsonConverter());
            }
        }
    }
}

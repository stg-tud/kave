using System;
using KaVE.EventGenerator.ReSharper8.Utils.Json;
using KaVE.Model.Events;
using Newtonsoft.Json;

namespace KaVE.EventGenerator.ReSharper8.SessionManager
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

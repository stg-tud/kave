using System.Runtime.Serialization;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class DebuggerEvent : IDEEvent
    {
        public enum DebuggerMode
        {
            Design,
            Run,
            Break,
            ExceptionThrown,
            ExceptionNotHandled
        }

        [DataMember]
        public DebuggerMode Mode { get; set; }

        [DataMember]
        public string Reason { get; set; }

        [DataMember]
        public string Action { get; set; }
    }
}
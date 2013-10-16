using System.Runtime.Serialization;
using CodeCompletion.Model.Events;

namespace KAVE.EventGenerator_VisualStudio10.Model
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
        public DebuggerMode Mode { get; internal set; }

        [DataMember]
        public string Reason { get; internal set; }

        [DataMember]
        public string Action { get; internal set; }
    }
}
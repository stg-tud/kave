using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class DebuggerEvent : IDEEvent
    {
        public const string EventKind = "Debugger";

        public enum DebuggerMode
        {
            Design,
            Run,
            Break,
            ExceptionThrown,
            ExceptionNotHandled
        }

        public DebuggerEvent() : base(EventKind) {}

        [DataMember]
        public DebuggerMode Mode { get; internal set; }

        [DataMember]
        public string Reason { get; internal set; }

        [DataMember]
        public string Action { get; internal set; }
    }
}
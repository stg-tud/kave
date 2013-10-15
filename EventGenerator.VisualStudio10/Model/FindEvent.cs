using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class FindEvent : IDEEvent
    {
        public const string EventKind = "Find";

        public FindEvent() : base(EventKind) {}

        [DataMember]
        public bool Cancelled { get; internal set; }
    }
}
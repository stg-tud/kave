using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class EditEvent : IDEEvent
    {
        public const string EventKind = "Edit";

        public EditEvent() : base(EventKind) {}

        [DataMember]
        public int NumberOfChanges { get; internal set; }

        [DataMember]
        public int SizeOfChanges { get; internal set; }
    }
}
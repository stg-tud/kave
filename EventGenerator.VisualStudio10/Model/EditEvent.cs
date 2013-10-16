using System.Runtime.Serialization;
using CodeCompletion.Model.Events;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class EditEvent : IDEEvent
    {
        [DataMember]
        public int NumberOfChanges { get; internal set; }

        [DataMember]
        public int SizeOfChanges { get; internal set; }
    }
}
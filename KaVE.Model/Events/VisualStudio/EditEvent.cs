using System.Runtime.Serialization;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class EditEvent : IDEEvent
    {
        [DataMember]
        public int NumberOfChanges { get; set; }

        [DataMember]
        public int SizeOfChanges { get; set; }
    }
}
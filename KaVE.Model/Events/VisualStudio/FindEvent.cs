using System.Runtime.Serialization;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class FindEvent : IDEEvent
    {
        [DataMember]
        public bool Cancelled { get; set; }
    }
}
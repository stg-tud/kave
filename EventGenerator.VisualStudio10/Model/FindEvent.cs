using System.Runtime.Serialization;
using CodeCompletion.Model.Events;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class FindEvent : IDEEvent
    {
        [DataMember]
        public bool Cancelled { get; internal set; }
    }
}
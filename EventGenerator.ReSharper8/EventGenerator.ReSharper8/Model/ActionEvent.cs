using System.Runtime.Serialization;
using CodeCompletion.Model.Events;

namespace EventGenerator.ReSharper8.Model
{
    [DataContract]
    public class ActionEvent : IDEEvent
    {
        [DataMember]
        public string ActionId { get; internal set; }

        [DataMember]
        public string ActionText { get; internal set; }
    }
}
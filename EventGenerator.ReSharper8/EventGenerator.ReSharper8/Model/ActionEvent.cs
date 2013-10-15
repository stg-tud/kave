using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace EventGenerator.ReSharper8.Model
{
    [DataContract]
    public class ActionEvent : IDEEvent
    {
        public const string EventKind = "ReSharper.Action";

        public ActionEvent() : base(EventKind) {}

        [DataMember]
        public string ActionId { get; internal set; }

        [DataMember]
        public string ActionText { get; internal set; }
    }
}
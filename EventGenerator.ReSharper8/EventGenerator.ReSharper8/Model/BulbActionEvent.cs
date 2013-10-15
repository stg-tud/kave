using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace EventGenerator.ReSharper8.Model
{
    [DataContract]
    public class BulbActionEvent : IDEEvent
    {
        public const string EventKind = "ReSharper.BulbAction";

        public BulbActionEvent() : base(EventKind) {}

        [DataMember]
        public string ActionId { get; internal set; }

        [DataMember]
        public string ActionText { get; internal set; }
    }
}
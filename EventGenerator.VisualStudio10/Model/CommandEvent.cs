using System.Runtime.Serialization;
using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class CommandEvent : IDEEvent
    {
        public const string EventKind = "Command";

        public CommandEvent() : base(EventKind) {}

        [DataMember]
        public CommandName Command { get; internal set; }

        [DataMember]
        public CommandBarControlName Source { get; internal set; }
    }
}
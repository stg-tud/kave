using System.Runtime.Serialization;
using CodeCompletion.Model.Events;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class CommandEvent : IDEEvent
    {
        [DataMember]
        public CommandName Command { get; internal set; }

        [DataMember]
        public CommandBarControlName Source { get; internal set; }
    }
}
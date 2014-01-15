using System.Runtime.Serialization;
using KaVE.Model.Names.VisualStudio;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class CommandEvent : IDEEvent
    {
        [DataMember]
        public CommandName Command { get; set; }

        [DataMember]
        public CommandBarControlName Source { get; set; }
    }
}
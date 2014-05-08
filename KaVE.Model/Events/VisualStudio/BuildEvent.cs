using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class BuildEvent : IDEEvent
    {
        public BuildEvent()
        {
            Targets = new List<BuildTarget>();
        }

        [DataMember]
        public string Scope { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public IList<BuildTarget> Targets { get; private set; }
    }

    [DataContract]
    public class BuildTarget
    {
        [DataMember]
        public string Project { get; set; }
        [DataMember]
        public string ProjectConfiguration { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public string SolutionConfiguration { get; set; }
        [DataMember]
        public DateTime? StartedAt { get; set; }
        [DataMember]
        public TimeSpan? Duration { get; set; }
        [DataMember]
        public bool Successful { get; set; }
    }
}

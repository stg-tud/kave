using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class BuildEvent : IDEEvent
    {
        public const string EventKind = "Build";

        public BuildEvent() : base(EventKind)
        {
            Targets = new List<BuildTarget>();
        }

        [DataMember]
        public string Scope { get; internal set; }

        [DataMember]
        public string Action { get; internal set; }

        [DataMember]
        public IList<BuildTarget> Targets { get; private set; }
    }

    public class BuildTarget
    {
        public string Project { get; set; }
        public string ProjectConfiguration { get; set; }
        public string Platform { get; set; }
        public string SolutionConfiguration { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public bool Successful { get; set; }
    }
}

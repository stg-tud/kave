using System.Collections.Generic;
using System.Runtime.Serialization;
using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class IDEStartupStateEvent : IDEEvent
    {
        public const string EventKind = "IDEState";

        public IDEStartupStateEvent() : base(EventKind) {}

        [DataMember]
        public IList<WindowName> OpenWindows { get; internal set; }

        [DataMember]
        public IList<DocumentName> OpenDocuments { get; internal set; }
    }
}
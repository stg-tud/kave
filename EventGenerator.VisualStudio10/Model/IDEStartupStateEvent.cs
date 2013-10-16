using System.Collections.Generic;
using System.Runtime.Serialization;
using CodeCompletion.Model.Events;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class IDEStartupStateEvent : IDEEvent
    {
        [DataMember]
        public IList<WindowName> OpenWindows { get; internal set; }

        [DataMember]
        public IList<DocumentName> OpenDocuments { get; internal set; }
    }
}
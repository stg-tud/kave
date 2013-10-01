using System.Collections.Generic;
using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    public class IDEStateEvent : IDEEvent
    {
        public const string EventKind = "IDEState";

        public IDEStateEvent() : base(EventKind) {}

        public IList<WindowName> OpenWindows { get; internal set; }
        public IList<DocumentName> OpenDocuments { get; internal set; }
    }
}
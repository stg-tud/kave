using System.Runtime.Serialization;
using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class DocumentEvent : IDEEvent
    {
        public const string EventKind = "Document";

        public enum DocumentAction
        {
            Opened,
            Saved,
            Closing
        }

        public DocumentEvent() : base(EventKind) {}

        [DataMember]
        public DocumentName DocumentName { get; internal set; }

        [DataMember]
        public DocumentAction Action { get; internal set; }
    }
}
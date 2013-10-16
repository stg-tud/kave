using System.Runtime.Serialization;
using CodeCompletion.Model.Events;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class DocumentEvent : IDEEvent
    {
        public enum DocumentAction
        {
            Opened,
            Saved,
            Closing
        }

        [DataMember]
        public DocumentName DocumentName { get; internal set; }

        [DataMember]
        public DocumentAction Action { get; internal set; }
    }
}
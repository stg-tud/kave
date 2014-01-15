using System.Runtime.Serialization;
using KaVE.Model.Names.VisualStudio;

namespace KaVE.Model.Events.VisualStudio
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
        public DocumentName DocumentName { get; set; }

        [DataMember]
        public DocumentAction Action { get; set; }
    }
}
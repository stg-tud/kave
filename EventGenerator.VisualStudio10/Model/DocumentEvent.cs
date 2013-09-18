using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    public class DocumentEvent : IDEEvent
    {
        public const string EventKind = "Document";

        public enum DocumentAction
        {
            Opened,
            Saved,
            Closing
        }

        public DocumentEvent() : base(EventKind)
        {
        }

        public string DocumentName { get; internal set; }

        public DocumentAction Action { get; internal set; }
    }
}
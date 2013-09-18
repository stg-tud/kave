using System.ComponentModel.Composition;
using EnvDTE;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class DocumentEventGenerator : VisualStudioEventGenerator
    {
        private DocumentEvents _documentEvents;

        protected override void Initialize()
        {
            _documentEvents = DTEEvents.DocumentEvents;
            _documentEvents.DocumentOpened += _documentEvents_DocumentOpened;
            _documentEvents.DocumentSaved += _documentEvents_DocumentSaved;
            _documentEvents.DocumentClosing += _documentEvents_DocumentClosing;
        }

        void _documentEvents_DocumentOpened(Document document)
        {
            Fire(document.FullName, DocumentEvent.DocumentAction.Opened);
        }

        void _documentEvents_DocumentSaved(Document document)
        {
            Fire(document.FullName, DocumentEvent.DocumentAction.Saved);
            // TODO maybe safe (diff) groum?
        }

        void _documentEvents_DocumentClosing(Document document)
        {
            Fire(document.FullName, DocumentEvent.DocumentAction.Closing);
        }

        private void Fire(string documentName, DocumentEvent.DocumentAction action)
        {
            var documentEvent = Create<DocumentEvent>();
            documentEvent.DocumentName = documentName;
            documentEvent.Action = action;
            Fire(documentEvent);
        }
    }
}
using System.ComponentModel.Composition;
using CodeCompletion.Model.Events.VisualStudio;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.KAVE_MessageBus.MessageBus;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof(VisualStudioEventGenerator))]
    internal class DocumentEventGenerator : VisualStudioEventGenerator
    {
        private DocumentEvents _documentEvents;

        public DocumentEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        public override void Initialize()
        {
            _documentEvents = DTEEvents.DocumentEvents;
            _documentEvents.DocumentOpened += _documentEvents_DocumentOpened;
            _documentEvents.DocumentSaved += _documentEvents_DocumentSaved;
            _documentEvents.DocumentClosing += _documentEvents_DocumentClosing;
        }

        void _documentEvents_DocumentOpened(Document document)
        {
            Fire(document, DocumentEvent.DocumentAction.Opened);
        }

        void _documentEvents_DocumentSaved(Document document)
        {
            Fire(document, DocumentEvent.DocumentAction.Saved);
            // TODO maybe safe (diff) groum?
        }

        void _documentEvents_DocumentClosing(Document document)
        {
            Fire(document, DocumentEvent.DocumentAction.Closing);
        }

        private void Fire(Document document, DocumentEvent.DocumentAction action)
        {
            var documentEvent = Create<DocumentEvent>();
            documentEvent.DocumentName = VsComponentNameFactory.GetNameOf(document);
            documentEvent.Action = action;
            Fire(documentEvent);
        }
    }
}
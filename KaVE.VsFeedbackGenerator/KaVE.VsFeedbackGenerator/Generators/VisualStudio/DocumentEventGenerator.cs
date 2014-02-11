using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Names;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class DocumentEventGenerator : AbstractEventGenerator
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DocumentEvents _documentEvents;

        public DocumentEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _documentEvents = DTE.Events.DocumentEvents;
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
            documentEvent.DocumentName = document.GetName();
            documentEvent.Action = action;
            FireNow(documentEvent);
        }
    }
}
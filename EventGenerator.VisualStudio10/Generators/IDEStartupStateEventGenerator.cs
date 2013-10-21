using System.ComponentModel.Composition;
using EnvDTE;
using EventGenerator.Commons;
using KaVE.MessageBus.MessageBus;
using KaVE.Model.Events.VisualStudio;

namespace KaVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class IDEStartupStateEventGenerator : VisualStudioEventGenerator
    {
        public IDEStartupStateEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        public override void Initialize()
        {
            // TODO defer this until IDE is actually loaded!
            // TODO add IDE shutdown event
            var ideStateEvent = Create<IDEStartupStateEvent>();
            ideStateEvent.OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows);
            ideStateEvent.OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents);
            Fire(ideStateEvent);
        }
    }
}

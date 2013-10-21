using EnvDTE;
using KaVE.EventGenerator.VisualStudio10.Utils;
using KaVE.MessageBus.MessageBus;
using KaVE.Model.Events.VisualStudio;

namespace KaVE.EventGenerator.VisualStudio10.Generators
{
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

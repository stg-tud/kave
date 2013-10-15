using System.ComponentModel.Composition;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Model;
using KAVE.KAVE_MessageBus.MessageBus;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class IDEStartupStateEventGenerator : VisualStudioEventGenerator
    {
        public IDEStartupStateEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        public override void Initialize()
        {
            var ideStateEvent = Create<IDEStartupStateEvent>();
            ideStateEvent.OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows);
            ideStateEvent.OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents);
            Fire(ideStateEvent);
        }
    }
}

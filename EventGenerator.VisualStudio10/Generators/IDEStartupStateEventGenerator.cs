using System.ComponentModel.Composition;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class IDEStartupStateEventGenerator : VisualStudioEventGenerator
    {
        public IDEStartupStateEventGenerator(DTE dte) : base(dte) {}

        public override void Initialize()
        {
            var ideStateEvent = Create<IDEStateEvent>();
            ideStateEvent.OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows);
            ideStateEvent.OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents);
            Fire(ideStateEvent);
        }
    }
}

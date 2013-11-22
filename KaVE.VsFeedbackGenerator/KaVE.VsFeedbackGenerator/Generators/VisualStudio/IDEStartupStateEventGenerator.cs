using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.EventGenerator.ReSharper8.MessageBus;
using KaVE.EventGenerator.VisualStudio10.Utils;
using KaVE.Model.Events.VisualStudio;

namespace KaVE.EventGenerator.ReSharper8.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class IDEStartupStateEventGenerator : AbstractEventGenerator
    {
        public IDEStartupStateEventGenerator(DTE dte, IMessageBus messageBus) : base(dte, messageBus)
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

using System;
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class IDEStartupStateEventGenerator : AbstractEventGenerator, IDisposable
    {
        public IDEStartupStateEventGenerator(DTE dte, IMessageBus messageBus) : base(dte, messageBus)
        {
            FireIDEStateEvent(IDEStateEvent.LifecyclePhase.Startup);
        }

        public void Dispose()
        {
            FireIDEStateEvent(IDEStateEvent.LifecyclePhase.Shutdown);
        }

        private void FireIDEStateEvent(IDEStateEvent.LifecyclePhase phase)
        {
            var ideStateEvent = Create<IDEStateEvent>();
            ideStateEvent.IDELifecyclePhase = phase;
            ideStateEvent.OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows);
            ideStateEvent.OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents);
            Fire(ideStateEvent);
        }
    }
}

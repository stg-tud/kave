using System;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils.Names;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class IDEStateEventGenerator : AbstractEventGenerator, IDisposable
    {
        public IDEStateEventGenerator(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
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
            ideStateEvent.OpenWindows = DTE.Windows.GetNames();
            ideStateEvent.OpenDocuments = DTE.Documents.GetNames();
            FireNow(ideStateEvent);
        }
    }
}

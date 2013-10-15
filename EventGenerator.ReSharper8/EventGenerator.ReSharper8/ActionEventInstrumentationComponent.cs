using System.Linq;
using EnvDTE;
using EventGenerator.ReSharper8.Generators;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.VsIntegration.Application;
using KAVE.KAVE_MessageBus.MessageBus;
using Microsoft.VisualStudio.OLE.Interop;

namespace EventGenerator.ReSharper8
{
    [ShellComponent]
    internal class ActionEventInstrumentationComponent
    {
        public ActionEventInstrumentationComponent(Lifetime lifetime, IActionManager actionManager, IServiceProvider serviceProvider)
        {
            var dte = serviceProvider.GetService<DTE, DTE>();
            var messageBus = serviceProvider.GetService<SMessageBus, SMessageBus>();

            foreach (var updatableAction in actionManager.GetAllActions().OfType<IUpdatableAction>())
            {
                updatableAction.AddHandler(lifetime, new EventGeneratingActionHandler(updatableAction, dte, messageBus));
            }
        }
    }
}
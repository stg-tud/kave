using System.Linq;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.DataFlow;
using KaVE.EventGenerator.ReSharper8.Generators;
using KaVE.EventGenerator.ReSharper8.VsIntegration;
using KaVE.MessageBus.MessageBus;

namespace KaVE.EventGenerator.ReSharper8
{
    [ShellComponent]
    internal class ActionEventInstrumentationComponent
    {
        public ActionEventInstrumentationComponent(Lifetime lifetime, IActionManager actionManager, IVsDTE dte, SMessageBus messageBus)
        {
            foreach (var updatableAction in actionManager.GetAllActions().OfType<IUpdatableAction>())
            {
                updatableAction.AddHandler(lifetime, new EventGeneratingActionHandler(updatableAction, dte.DTE, messageBus));
            }
        }
    }
}
using System.Linq;
using EnvDTE;
using EventGenerator.ReSharper8.Generators;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using JetBrains.VsIntegration.Application;
using KaVE.MessageBus.MessageBus;

namespace EventGenerator.ReSharper8
{
    [ShellComponent]
    internal class ActionEventInstrumentationComponent
    {
        public ActionEventInstrumentationComponent(Lifetime lifetime, IActionManager actionManager, RawVsServiceProvider serviceProvider)
        {
            var dte = serviceProvider.Value.GetService<DTE, DTE>();
            var messageBus = serviceProvider.Value.GetService<SMessageBus, SMessageBus>();

            foreach (var updatableAction in actionManager.GetAllActions().OfType<IUpdatableAction>())
            {
                updatableAction.AddHandler(lifetime, new EventGeneratingActionHandler(updatableAction, dte, messageBus));
            }
        }
    }
}
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using KaVE.EventGenerator.ReSharper8.Generators;
using KaVE.EventGenerator.ReSharper8.Generators.ReSharper;
using KaVE.EventGenerator.ReSharper8.MessageBus;
using KaVE.EventGenerator.ReSharper8.VsIntegration;

namespace KaVE.EventGenerator.ReSharper8
{
    [ShellComponent]
    internal class ActionEventInstrumentationComponent
    {
        public ActionEventInstrumentationComponent(Lifetime lifetime, IActionManager actionManager, IVsDTE dte, IMessageBus messageBus)
        {
            foreach (var updatableAction in actionManager.GetAllActions().OfType<IUpdatableAction>())
            {
                updatableAction.AddHandler(lifetime, new EventGeneratingActionHandler(updatableAction, dte, messageBus));
            }
        }
    }
}
using System.Linq;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;

namespace EventGenerator.ReSharper8.Generators
{
    [ShellComponent]
    internal class ActionEventHandlerRegistrationComponent
    {
        public ActionEventHandlerRegistrationComponent(Lifetime lifetime, IActionManager actionManager, DTE dte)
        {
            foreach (var updatableAction in actionManager.GetAllActions().OfType<IUpdatableAction>())
            {
                updatableAction.AddHandler(lifetime, new EventGeneratingActionHandler(updatableAction, dte));
            }
        }
    }
}
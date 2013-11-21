using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using KaVE.EventGenerator.ReSharper8.MessageBus;
using KaVE.Model.Events.ReSharper;

namespace KaVE.EventGenerator.ReSharper8.Generators.ReSharper
{
    /// <summary>
    /// Fires an <see cref="ActionEvent"/> on execution of a ReSharper action. Passes handling of the action on the
    /// the default handler.
    /// </summary>
    internal class EventGeneratingActionHandler : AbstractEventGenerator, IActionHandler
    {
        private readonly IUpdatableAction _updatableAction;

        public EventGeneratingActionHandler(IUpdatableAction updatableAction, DTE dte, IMessageBus messageBus)
            : base(dte, messageBus)
        {
            _updatableAction = updatableAction;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return nextUpdate.Invoke();
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var actionEvent = Create<ActionEvent>();
            actionEvent.ActionId = _updatableAction.Id;
            actionEvent.ActionText = _updatableAction.Presentation.Text;
            Fire(actionEvent);

            nextExecute.Invoke();
        }
    }
}
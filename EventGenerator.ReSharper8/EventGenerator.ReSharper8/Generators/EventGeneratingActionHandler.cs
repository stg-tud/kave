using EnvDTE;
using EventGenerator.Commons;
using EventGenerator.ReSharper8.Model;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace EventGenerator.ReSharper8.Generators
{
    /// <summary>
    /// Fires an <see cref="ActionEvent"/> on execution of a ReSharper action. Passes handling of the action on the
    /// the default handler.
    /// </summary>
    internal class EventGeneratingActionHandler : AbstractEventGenerator, IActionHandler
    {
        private readonly IUpdatableAction _updatableAction;
        private readonly DTE _dte;

        public EventGeneratingActionHandler(IUpdatableAction updatableAction, DTE dte)
        {
            _updatableAction = updatableAction;
            _dte = dte;
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

        protected override DTE DTE
        {
            get { return _dte; }
        }
    }
}
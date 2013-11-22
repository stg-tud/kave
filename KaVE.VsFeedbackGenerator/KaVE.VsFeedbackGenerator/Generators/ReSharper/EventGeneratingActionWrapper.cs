using System;
using EnvDTE;
using KaVE.EventGenerator.ReSharper8.MessageBus;
using KaVE.Model.Events.ReSharper;

namespace KaVE.EventGenerator.ReSharper8.Generators.ReSharper
{
    internal class EventGeneratingActionWrapper : AbstractEventGenerator
    {
        private readonly Action _originalAction;

        public EventGeneratingActionWrapper(Action originalAction, DTE dte, IMessageBus messageBus)
            : base(dte, messageBus)
        {
            _originalAction = originalAction;
        }

        public void Execute()
        {
            var actionEvent = Create<BulbActionEvent>();
            actionEvent.ActionId = _originalAction.Target.ToString();
            _originalAction.Invoke();
            Fire(actionEvent);
        }
    }
}
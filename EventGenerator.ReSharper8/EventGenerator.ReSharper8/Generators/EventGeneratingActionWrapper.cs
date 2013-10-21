using System;
using EnvDTE;
using KaVE.EventGenerator.VisualStudio10.Generators;
using KaVE.MessageBus.MessageBus;
using KaVE.Model.Events.ReSharper;

namespace KaVE.EventGenerator.ReSharper8.Generators
{
    internal class EventGeneratingActionWrapper : AbstractEventGenerator
    {
        private readonly Action _originalAction;

        public EventGeneratingActionWrapper(Action originalAction, DTE dte, SMessageBus messageBus)
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
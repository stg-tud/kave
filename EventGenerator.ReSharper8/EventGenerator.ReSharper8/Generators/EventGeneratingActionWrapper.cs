using System;
using CodeCompletion.Model.Events.ReSharper;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.KAVE_MessageBus.MessageBus;

namespace EventGenerator.ReSharper8.Generators
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
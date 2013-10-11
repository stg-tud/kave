using System;
using EnvDTE;
using EventGenerator.Commons;
using EventGenerator.ReSharper8.Model;

namespace EventGenerator.ReSharper8.Generators
{
    internal class EventGeneratingActionWrapper : AbstractEventGenerator
    {
        private readonly Action _originalAction;

        public EventGeneratingActionWrapper(Action originalAction, DTE dte) : base(dte)
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
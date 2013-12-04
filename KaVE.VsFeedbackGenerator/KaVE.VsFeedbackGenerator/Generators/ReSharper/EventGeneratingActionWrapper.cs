using System;
using KaVE.Model.Events.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal class EventGeneratingActionWrapper : AbstractEventGenerator
    {
        private readonly Action _originalAction;

        public EventGeneratingActionWrapper(Action originalAction, IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _originalAction = originalAction;
        }

        public void Execute()
        {
            var actionEvent = Create<BulbActionEvent>();
            actionEvent.ActionId = _originalAction.Target.ToString();
            _originalAction.Invoke();
            FireNow(actionEvent);
        }
    }
}
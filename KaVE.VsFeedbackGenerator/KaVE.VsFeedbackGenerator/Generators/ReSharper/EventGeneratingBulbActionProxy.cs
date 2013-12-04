using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.TextControl;
using KaVE.Model.Events.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal class EventGeneratingBulbActionProxy : AbstractEventGenerator, IBulbAction
    {
        private readonly IBulbAction _target;

        public EventGeneratingBulbActionProxy(IBulbAction target, IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _target = target;
        }

        public void Execute(ISolution solution, ITextControl textControl)
        {
            var bulbActionEvent = Create<BulbActionEvent>();
            bulbActionEvent.ActionId = _target.GetType().FullName;
            bulbActionEvent.ActionText = Text;
            _target.Execute(solution, textControl);
            FireNow(bulbActionEvent);
        }

        public string Text
        {
            get { return _target.Text; }
        }
    }
}
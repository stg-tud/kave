using EnvDTE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.TextControl;
using KaVE.Model.Events.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal class EventGeneratingBulbActionProxy : AbstractEventGenerator, IBulbAction
    {
        private readonly IBulbAction _target;

        public EventGeneratingBulbActionProxy(IBulbAction target, DTE dte, IMessageBus messageBus)
            : base(dte, messageBus)
        {
            _target = target;
        }

        public void Execute(ISolution solution, ITextControl textControl)
        {
            var bulbActionEvent = Create<BulbActionEvent>();
            bulbActionEvent.ActionId = _target.GetType().FullName;
            bulbActionEvent.ActionText = Text;
            _target.Execute(solution, textControl);
            Fire(bulbActionEvent);
        }

        public string Text
        {
            get { return _target.Text; }
        }
    }
}
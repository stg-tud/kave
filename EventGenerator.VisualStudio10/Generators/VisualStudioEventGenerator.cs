using EnvDTE;
using JetBrains.Annotations;
using KaVE.MessageBus.MessageBus;

namespace KaVE.EventGenerator.VisualStudio10.Generators
{
    internal abstract class VisualStudioEventGenerator : AbstractEventGenerator
    {
        protected VisualStudioEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        [NotNull]
        protected Events DTEEvents
        {
            get { return DTE.Events; }
        }

        public abstract void Initialize();
    }
}
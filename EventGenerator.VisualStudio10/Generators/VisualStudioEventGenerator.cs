using EnvDTE;
using EventGenerator.Commons;
using JetBrains.Annotations;
using KAVE.KAVE_MessageBus.MessageBus;

namespace KAVE.EventGenerator_VisualStudio10.Generators
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
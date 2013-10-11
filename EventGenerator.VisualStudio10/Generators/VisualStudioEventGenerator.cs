using CodeCompletion.Utils;
using EnvDTE;
using EventGenerator.Commons;
using JetBrains.Annotations;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    internal abstract class VisualStudioEventGenerator : AbstractEventGenerator
    {
        protected VisualStudioEventGenerator(DTE dte) : base(dte) {}

        [NotNull]
        protected Events DTEEvents
        {
            get { return DTE.Events; }
        }

        public abstract void Initialize();
    }
}
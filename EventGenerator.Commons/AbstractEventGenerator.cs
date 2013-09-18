using CodeCompletion.Model;
using EnvDTE;

namespace EventGenerator.Commons
{
    public abstract class AbstractEventGenerator
    {
        protected abstract DTE DTE { get; }

        protected TIDEEvent Create<TIDEEvent>() where TIDEEvent : IDEEvent, new()
        {
            return new TIDEEvent
            {
                ActiveWindow = VsComponentNameFactory.GetNameOf(DTE.ActiveWindow),
                ActiveDocument = VsComponentNameFactory.GetNameOf(DTE.ActiveDocument),
                OpenWindows = VsComponentNameFactory.GetNamesOf(DTE.Windows),
                OpenDocuments = VsComponentNameFactory.GetNamesOf(DTE.Documents),
                OpenSolution = VsComponentNameFactory.GetNameOf(DTE.Solution)
            };
        }


    }
}

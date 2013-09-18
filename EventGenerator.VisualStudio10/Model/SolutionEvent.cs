using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    class SolutionEvent : IDEEvent
    {
        public const string EventKind = "Solution";

        public enum SolutionAction
        {
            OpenSolution,
            RenameSolution,
            CloseSolution,
            AddSolutionItem,
            RenameSolutionItem,
            RemoveSolutionItem,
            AddProject,
            RenameProject,
            RemoveProject,
            AddProjectItem,
            RenameProjectItem,
            RemoveProjectItem
        }

        public SolutionEvent()
            : base(EventKind)
        {
        }

        public SolutionAction Action { get; internal set; }

        public IIDEComponentName Target { get; internal set; }
    }
}

using System.Runtime.Serialization;
using CodeCompletion.Model.Events;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class SolutionEvent : IDEEvent
    {
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

        [DataMember]
        public SolutionAction Action { get; internal set; }

        [DataMember]
        public IIDEComponentName Target { get; internal set; }
    }
}
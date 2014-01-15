using System.Runtime.Serialization;
using KaVE.Model.Names.VisualStudio;

namespace KaVE.Model.Events.VisualStudio
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
        public SolutionAction Action { get; set; }

        [DataMember]
        public IIDEComponentName Target { get; set; }
    }
}
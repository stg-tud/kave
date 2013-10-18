using System.ComponentModel.Composition;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.KAVE_MessageBus.MessageBus;
using KaVE.Model.Events.VisualStudio;
using KaVE.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class SolutionEventGenerator : VisualStudioEventGenerator
    {
        private SolutionEvents _solutionEvents;
        private ProjectItemsEvents _solutionItemsEvents;
        private ProjectItemsEvents _projectItemsEvents;
        private SelectionEvents _selectionEvents;

        public SolutionEventGenerator(DTE dte, SMessageBus messageBus) : base(dte, messageBus) {}

        public override void Initialize()
        {
            _solutionEvents = DTEEvents.SolutionEvents;
            _solutionEvents.Opened += _solutionEvents_Opened;
            _solutionEvents.ProjectAdded += _solutionEvents_ProjectAdded;
            _solutionEvents.ProjectRenamed += _solutionEvents_ProjectRenamed;
            _solutionEvents.ProjectRemoved += _solutionEvents_ProjectRemoved;
            _solutionEvents.Renamed += _solutionEvents_Renamed;
            _solutionEvents.BeforeClosing += _solutionEvents_BeforeClosing;

            _solutionItemsEvents = DTEEvents.SolutionItemsEvents;
            _solutionItemsEvents.ItemAdded += _solutionItemsEvents_ItemAdded;
            _solutionItemsEvents.ItemRenamed += _solutionItemsEvents_ItemRenamed;
            _solutionItemsEvents.ItemRemoved += _solutionItemsEvents_ItemRemoved;

            _projectItemsEvents = DTEEvents.MiscFilesEvents;
            _projectItemsEvents.ItemAdded += _projectItemsEvents_ItemAdded;
            _projectItemsEvents.ItemRenamed += _projectItemsEvents_ItemRenamed;
            _projectItemsEvents.ItemRemoved += _projectItemsEvents_ItemRemoved;

            _selectionEvents = DTEEvents.SelectionEvents;
            _selectionEvents.OnChange += _selectionEvents_OnChange;
        }

        private void _solutionItemsEvents_ItemAdded(ProjectItem projectItem)
        {
            Fire(SolutionEvent.SolutionAction.AddSolutionItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        private void _solutionItemsEvents_ItemRenamed(ProjectItem projectItem, string oldName)
        {
            Fire(SolutionEvent.SolutionAction.RenameSolutionItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        private void _solutionItemsEvents_ItemRemoved(ProjectItem projectItem)
        {
            Fire(SolutionEvent.SolutionAction.RemoveSolutionItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        private void _solutionEvents_Opened()
        {
            Fire(SolutionEvent.SolutionAction.OpenSolution, VsComponentNameFactory.GetNameOf(DTE.Solution));
        }

        private void _solutionEvents_ProjectAdded(Project project)
        {
            Fire(SolutionEvent.SolutionAction.AddProject, VsComponentNameFactory.GetNameOf(project));
        }

        private void _projectItemsEvents_ItemAdded(ProjectItem projectItem)
        {
            Fire(SolutionEvent.SolutionAction.AddProjectItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        private void _projectItemsEvents_ItemRenamed(ProjectItem projectItem, string oldName)
        {
            Fire(SolutionEvent.SolutionAction.RenameProjectItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        private void _projectItemsEvents_ItemRemoved(ProjectItem projectItem)
        {
            Fire(SolutionEvent.SolutionAction.RemoveProjectItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        private void _solutionEvents_ProjectRenamed(Project project, string oldName)
        {
            Fire(SolutionEvent.SolutionAction.RenameProject, VsComponentNameFactory.GetNameOf(project));
        }

        private void _solutionEvents_ProjectRemoved(Project project)
        {
            Fire(SolutionEvent.SolutionAction.RemoveProject, VsComponentNameFactory.GetNameOf(project));
        }

        private void _solutionEvents_Renamed(string oldName)
        {
            Fire(SolutionEvent.SolutionAction.RenameSolution, VsComponentNameFactory.GetNameOf(DTE.Solution));
        }

        private void _solutionEvents_BeforeClosing()
        {
            Fire(SolutionEvent.SolutionAction.CloseSolution, VsComponentNameFactory.GetNameOf(DTE.Solution));
        }

        private void _selectionEvents_OnChange()
        {
            // TODO fire project item selection event?
            // this method behaves strange... e.g., selection in solution explorer is recognized. Adding to that
            // selection by ctrl+click is recognized. Any further additions to the selection, using ctrl+click, are
            // not recognized.
        }

        private void Fire(SolutionEvent.SolutionAction action, IIDEComponentName target)
        {
            var solutionEvent = Create<SolutionEvent>();
            solutionEvent.Action = action;
            solutionEvent.Target = target;
            Fire(solutionEvent);
        }
    }
}
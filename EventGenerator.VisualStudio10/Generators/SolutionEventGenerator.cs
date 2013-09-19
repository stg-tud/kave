using System.Collections.Generic;
using System.ComponentModel.Composition;
using CodeCompletion.Model.Names.VisualStudio;
using EnvDTE;
using EventGenerator.Commons;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class SolutionEventGenerator : VisualStudioEventGenerator
    {
        private SolutionEvents _solutionEvents;
        private ProjectItemsEvents _solutionItemsEvents;
        private ProjectItemsEvents _projectItemsEvents;
        private SelectionEvents _selectionEvents;

        protected override void Initialize()
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

        void _solutionItemsEvents_ItemAdded(ProjectItem projectItem)
        {
            Fire(SolutionEvent.SolutionAction.AddSolutionItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        void _solutionItemsEvents_ItemRenamed(ProjectItem projectItem, string oldName)
        {
            Fire(SolutionEvent.SolutionAction.RenameSolutionItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        void _solutionItemsEvents_ItemRemoved(ProjectItem projectItem)
        {
            Fire(SolutionEvent.SolutionAction.RemoveSolutionItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        void _solutionEvents_Opened()
        {
            Fire(SolutionEvent.SolutionAction.OpenSolution, VsComponentNameFactory.GetNameOf(DTE.Solution));
        }

        void _solutionEvents_ProjectAdded(Project project)
        {
            Fire(SolutionEvent.SolutionAction.AddProject, VsComponentNameFactory.GetNameOf(project));
        }

        void _projectItemsEvents_ItemAdded(ProjectItem projectItem)
        {
            // TODO finish add events
            Fire(SolutionEvent.SolutionAction.AddProjectItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        void _projectItemsEvents_ItemRenamed(ProjectItem projectItem, string oldName)
        {
            // TODO finish rename events
            Fire(SolutionEvent.SolutionAction.RenameProjectItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        void _projectItemsEvents_ItemRemoved(ProjectItem projectItem)
        {
            // TODO finish remove events
            Fire(SolutionEvent.SolutionAction.RemoveProjectItem, VsComponentNameFactory.GetNameOf(projectItem));
        }

        void _solutionEvents_ProjectRenamed(Project project, string oldName)
        {
            Fire(SolutionEvent.SolutionAction.RenameProject, VsComponentNameFactory.GetNameOf(project));
        }

        void _solutionEvents_ProjectRemoved(Project project)
        {
            Fire(SolutionEvent.SolutionAction.RemoveProject, VsComponentNameFactory.GetNameOf(project));
        }

        void _solutionEvents_Renamed(string oldName)
        {
            Fire(SolutionEvent.SolutionAction.RenameSolution, VsComponentNameFactory.GetNameOf(DTE.Solution));
        }

        void _solutionEvents_BeforeClosing()
        {
            Fire(SolutionEvent.SolutionAction.CloseSolution, VsComponentNameFactory.GetNameOf(DTE.Solution));
        }

        private void _selectionEvents_OnChange()
        {
            // TODO fire project item selection event?
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
using System.ComponentModel.Composition;
using EnvDTE;
using KAVE.EventGenerator_VisualStudio10.Model;

namespace KAVE.EventGenerator_VisualStudio10.Generators
{
    [Export(typeof (VisualStudioEventGenerator))]
    internal class TaskListEventGenerator : VisualStudioEventGenerator
    {
        private TaskListEvents _taskListEvents;

        public TaskListEventGenerator(DTE dte) : base(dte) {}

        public override void Initialize()
        {
            _taskListEvents = DTEEvents.TaskListEvents;
            _taskListEvents.TaskAdded += _taskListEvents_TaskAdded;
            _taskListEvents.TaskModified += _taskListEvents_TaskModified;
            _taskListEvents.TaskNavigated += _taskListEvents_TaskNavigated;
            _taskListEvents.TaskRemoved += _taskListEvents_TaskRemoved;
        }

        void _taskListEvents_TaskAdded(TaskItem taskItem)
        {
            Fire(TaskListEvent.TaskListAction.AddTask);
        }

        void _taskListEvents_TaskModified(TaskItem taskItem, vsTaskListColumn columnModified)
        {
            Fire(TaskListEvent.TaskListAction.ModifyTask);
        }

        void _taskListEvents_TaskNavigated(TaskItem taskItem, ref bool navigateHandled)
        {
            Fire(TaskListEvent.TaskListAction.NavigateTask);
        }

        void _taskListEvents_TaskRemoved(TaskItem taskItem)
        {
            Fire(TaskListEvent.TaskListAction.RemoveTask);
        }

        private void Fire(TaskListEvent.TaskListAction action)
        {
            var taskListEvent = Create<TaskListEvent>();
            taskListEvent.Action = action;
            Fire(taskListEvent);
        }
    }
}
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.MessageBus;

namespace KaVE.VsFeedbackGenerator.Generators.VisualStudio
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    internal class TaskListEventGenerator : AbstractEventGenerator
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly TaskListEvents _taskListEvents;

        public TaskListEventGenerator(DTE dte, IMessageBus messageBus) : base(dte, messageBus)
        {
            _taskListEvents = DTE.Events.TaskListEvents;
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
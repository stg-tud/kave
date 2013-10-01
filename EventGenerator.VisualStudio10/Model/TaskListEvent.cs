using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    internal class TaskListEvent : IDEEvent
    {
        public const string EventKind = "TaskList";

        public enum TaskListAction
        {
            AddTask,
            ModifyTask,
            NavigateTask,
            RemoveTask
        }

        public TaskListEvent() : base(EventKind) {}

        public TaskListAction Action { get; internal set; }
    }
}
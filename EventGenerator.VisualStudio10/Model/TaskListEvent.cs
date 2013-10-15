using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class TaskListEvent : IDEEvent
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

        [DataMember]
        public TaskListAction Action { get; internal set; }
    }
}
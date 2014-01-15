using System.Runtime.Serialization;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class TaskListEvent : IDEEvent
    {
        public enum TaskListAction
        {
            AddTask,
            ModifyTask,
            NavigateTask,
            RemoveTask
        }

        [DataMember]
        public TaskListAction Action { get; set; }
    }
}
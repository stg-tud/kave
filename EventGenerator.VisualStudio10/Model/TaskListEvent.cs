using System.Runtime.Serialization;
using CodeCompletion.Model.Events;

namespace KAVE.EventGenerator_VisualStudio10.Model
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
        public TaskListAction Action { get; internal set; }
    }
}
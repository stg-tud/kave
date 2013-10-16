using System.Runtime.Serialization;
using CodeCompletion.Model;
using CodeCompletion.Model.Events;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class WindowEvent : IDEEvent
    {
        public enum WindowAction
        {
            Create,
            Activate,
            Move,
            Close
        }

        [DataMember]
        public WindowName Window { get; internal set; }

        [DataMember]
        public WindowAction Action { get; internal set; }
    }
}
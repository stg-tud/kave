using System.Runtime.Serialization;
using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class WindowEvent : IDEEvent
    {
        public const string EventKind = "Window";

        public enum WindowAction
        {
            Create,
            Activate,
            Move,
            Close
        }

        public WindowEvent() : base(EventKind) {}

        [DataMember]
        public WindowName Window { get; internal set; }

        [DataMember]
        public WindowAction Action { get; internal set; }
    }
}
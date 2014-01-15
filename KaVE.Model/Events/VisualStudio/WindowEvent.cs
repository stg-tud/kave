using System.Runtime.Serialization;
using KaVE.Model.Names.VisualStudio;

namespace KaVE.Model.Events.VisualStudio
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
        public WindowName Window { get; set; }

        [DataMember]
        public WindowAction Action { get; set; }
    }
}
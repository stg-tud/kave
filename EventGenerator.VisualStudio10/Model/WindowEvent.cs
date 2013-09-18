using CodeCompletion.Model;
using CodeCompletion.Model.Names.VisualStudio;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
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

        public WindowEvent() : base(EventKind)
        {
        }

        public WindowName Window { get; internal set; }
        public WindowAction Action { get; internal set; }
    }
}

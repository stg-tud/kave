using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    class DebuggerEvent : IDEEvent
    {
        public const string EventKind = "Debugger";

        public enum DebuggerMode
        {
            Design,
            Run,
            Break,
            ExceptionThrown,
            ExceptionNotHandled
        }

        public DebuggerEvent() : base(EventKind)
        {
        }

        public DebuggerMode Mode { get; internal set; }
        public string Reason { get; internal set; }
        public string Action { get; internal set; }
    }
}

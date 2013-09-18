using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    class IDEModeEvent : IDEEvent
    {
        public const string EventKind = "IDE";

        public enum IDEMode
        {
            Debug,
            Design
        }

        public IDEModeEvent() : base(EventKind)
        {
        }

        public IDEMode SwitchTo { get; internal set; }
    }
}

using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    class EditEvent : IDEEvent
    {
        public const string EventKind = "Edit";

        public EditEvent() : base(EventKind)
        {
        }

        public int Line { get; internal set; }

        public int ChangeSize { get; internal set; }
    }
}

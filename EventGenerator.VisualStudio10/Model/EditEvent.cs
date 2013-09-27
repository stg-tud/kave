using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    class EditEvent : IDEEvent
    {
        public const string EventKind = "Edit";

        public EditEvent() : base(EventKind)
        {
        }

        public int NumberOfChangedLines { get; internal set; }

        public int NumberOfChangedCharacters { get; internal set; }
    }
}

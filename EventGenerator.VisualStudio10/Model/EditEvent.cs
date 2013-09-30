using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    class EditEvent : IDEEvent
    {
        public const string EventKind = "Edit";

        public EditEvent() : base(EventKind)
        {
        }

        public int NumberOfChanges { get; internal set; }

        public int SizeOfChanges { get; internal set; }
    }
}

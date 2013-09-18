using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    class FindEvent : IDEEvent
    {
        public const string EventKind = "Find";

        public FindEvent() : base(EventKind)
        {
        }



        public bool Cancelled { get; internal set; }
    }
}

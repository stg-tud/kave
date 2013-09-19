using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    public class OutputWindowEvent : IDEEvent
    {
        public const string EventKind = "OutputWindow";

        public enum OutputWindowAction
        {
            AddPane,
            UpdatePane,
            ClearPane
        }

        public OutputWindowEvent()
            : base(EventKind)
        {
        }

        public OutputWindowAction Action { get; internal set; }

        public string PaneName { get; internal set; }
    }
}
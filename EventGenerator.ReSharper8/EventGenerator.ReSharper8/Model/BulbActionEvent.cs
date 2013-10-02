using CodeCompletion.Model;

namespace EventGenerator.ReSharper8.Model
{
    public class BulbActionEvent : IDEEvent
    {
        public const string EventKind = "ReSharper.BulbAction";

        public BulbActionEvent() : base(EventKind) {}

        public string ActionId { get; internal set; }
        public string ActionText { get; internal set; }
    }
}
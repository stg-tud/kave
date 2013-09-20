using CodeCompletion.Model;

namespace EventGenerator.ReSharper8.Model
{
    public class ActionEvent : IDEEvent
    {
        public const string EventKind = "ReSharper.Action";

        public ActionEvent() : base(EventKind)
        {
            
        }

        public string ActionId { get; internal set; }
        public string ActionText { get; internal set; }
    }
}

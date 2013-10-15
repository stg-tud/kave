using System.Runtime.Serialization;
using CodeCompletion.Model;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class OutputWindowEvent : IDEEvent
    {
        public const string EventKind = "OutputWindow";

        public enum OutputWindowAction
        {
            AddPane,
            UpdatePane,
            ClearPane
        }

        public OutputWindowEvent() : base(EventKind) {}

        [DataMember]
        public OutputWindowAction Action { get; internal set; }

        [DataMember]
        public string PaneName { get; internal set; }
    }
}
using System.Runtime.Serialization;
using CodeCompletion.Model.Events;

namespace KAVE.EventGenerator_VisualStudio10.Model
{
    [DataContract]
    public class OutputWindowEvent : IDEEvent
    {
        public enum OutputWindowAction
        {
            AddPane,
            UpdatePane,
            ClearPane
        }

        [DataMember]
        public OutputWindowAction Action { get; internal set; }

        [DataMember]
        public string PaneName { get; internal set; }
    }
}
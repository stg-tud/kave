using System.Runtime.Serialization;

namespace KaVE.Model.Events.VisualStudio
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
        public OutputWindowAction Action { get; set; }

        [DataMember]
        public string PaneName { get; set; }
    }
}
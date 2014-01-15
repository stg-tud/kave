using System.Runtime.Serialization;

namespace KaVE.Model.Events.ReSharper
{
    [DataContract]
    public class ActionEvent : IDEEvent
    {
        [DataMember]
        public string ActionId { get; set; }

        [DataMember]
        public string ActionText { get; set; }
    }
}
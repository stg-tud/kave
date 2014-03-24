using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    public class Notification
    {
        public string Caption { get; set; }
        public string Message { get; set; }

        protected bool Equals(Notification other)
        {
            return string.Equals(Message, other.Message) && string.Equals(Caption, other.Caption);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Message != null ? Message.GetHashCode() : 0)*397) ^
                       (Caption != null ? Caption.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("[Caption: {1}, Message: {0}]", Caption, Message);
        }
    }
}
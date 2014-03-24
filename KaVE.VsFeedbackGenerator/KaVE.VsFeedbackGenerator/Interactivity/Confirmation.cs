using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    public class Confirmation : Notification
    {
        public bool Confirmed { get; set; }

        protected bool Equals(Confirmation other)
        {
            return base.Equals(other) && Confirmed.Equals(other.Confirmed);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ Confirmed.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}, Confirmed: {1}]", base.ToString(), Confirmed);
        }
    }
}
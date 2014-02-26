using KaVE.Utils;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    class SerializationTestTarget
    {
        public string Id { get; set; }

        private bool Equals(SerializationTestTarget other)
        {
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0)*397);
            }
        }
    }
}
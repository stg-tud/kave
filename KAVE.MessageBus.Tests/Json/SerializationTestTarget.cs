namespace KAVE.MessageBus.Tests.Json
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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SerializationTestTarget) obj);
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
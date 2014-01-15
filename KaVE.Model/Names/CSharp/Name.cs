using System.Runtime.Serialization;
using KaVE.Utils.Collections;

namespace KaVE.Model.Names.CSharp
{
    [DataContract]
    public class Name : IName
    {
        private static readonly WeakReferenceDictionary<string, Name> NameRegistry = new WeakReferenceDictionary<string, Name>();

        public static Name Get(string identifier)
        {
            if (!NameRegistry.ContainsKey(identifier))
            {
                NameRegistry.Add(identifier, new Name(identifier));
            }
            return NameRegistry[identifier];
        }

        [DataMember(Order=1)]
        public string Identifier { get; private set; }

        protected Name(string identifier)
        {
            Identifier = identifier;
        }

        public override bool Equals(object other)
        {
            var otherName = other as IName;
            return otherName != null && Equals(otherName);
        }

        private bool Equals(IName other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }

        public static bool operator ==(Name n1, IName n2)
        {
            return Equals(n1, n2);
        }

        public static bool operator !=(Name n1, IName n2)
        {
            return !(n1 == n2);
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}

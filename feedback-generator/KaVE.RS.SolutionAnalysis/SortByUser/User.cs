using System.Collections.Generic;

namespace KaVE.RS.SolutionAnalysis.SortByUser
{
    public class User
    {
        public UserIdentifiers Identifiers { get; set; }
        public ISet<string> Files { get; private set; }

        public User()
        {
            Identifiers = new UserIdentifiers();
            Files = new HashSet<string>();
        }

        protected bool Equals(User other)
        {
            return Equals(Identifiers, other.Identifiers) && Files.SetEquals(other.Files);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifiers != null ? Identifiers.GetHashCode() : 0)*397) ^ (Files != null ? Files.GetHashCode() : 0);
            }
        }
    }
}
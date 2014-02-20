using System.Collections.Generic;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;

namespace KaVE.Model.Events.CompletionEvent
{
    public class TypeHierarchy : ITypeHierarchy
    {
        private class HierarchyComparer : IEqualityComparer<ITypeHierarchy>
        {
            public bool Equals(ITypeHierarchy x, ITypeHierarchy y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(ITypeHierarchy obj)
            {
                return obj.GetHashCode();
            }
        }

        private static readonly HierarchyComparer Comparer = new HierarchyComparer();

        public TypeHierarchy()
        {
            Implements = new HashSet<ITypeHierarchy>(Comparer);
        }

        public ITypeName Element { get; set; }
        public ITypeHierarchy Extends { get; set; }

        [NotNull]
        public ISet<ITypeHierarchy> Implements { get; set; }

        protected bool Equals(TypeHierarchy other)
        {
            var setse = Implements.Count == other.Implements.Count && Implements.All(iface => other.Implements.Contains(iface));
            var b = Equals(Extends, other.Extends);
            var equals1 = Equals(Element, other.Element);
            return setse && b && equals1;
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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((TypeHierarchy) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Implements.GetHashCode());
                hashCode = (hashCode*397) ^ (Extends != null ? Extends.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Element != null ? Element.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("[Element: {0}, Extends: {1}, Implements: {2}]", Element, Extends, Implements);
        }
    }
}

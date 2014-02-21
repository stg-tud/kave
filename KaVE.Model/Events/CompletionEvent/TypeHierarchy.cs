using System.Collections.Generic;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Utils.Collections;

namespace KaVE.Model.Events.CompletionEvent
{
    public class TypeHierarchy : ITypeHierarchy
    {
        private static readonly SetEqualityComparer<ITypeHierarchy> Comparer = new SetEqualityComparer<ITypeHierarchy>();

        public TypeHierarchy()
        {
            Implements = new HashSet<ITypeHierarchy>();
        }

        public ITypeName Element { get; set; }
        public ITypeHierarchy Extends { get; set; }

        [NotNull]
        public ISet<ITypeHierarchy> Implements { get; set; }

        protected bool Equals(TypeHierarchy other)
        {
            return Comparer.Equals(Implements, other.Implements) && Equals(Extends, other.Extends) && Equals(Element, other.Element);
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
                var hashCode = Comparer.GetHashCode(Implements);
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

using System;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;
using KaVE.Utils.Collections;

namespace KaVE.Model.Events.CompletionEvent
{
    public class TypeHierarchy : ITypeHierarchy
    {
        private static readonly SetEqualityComparer<ITypeHierarchy> Comparer = new SetEqualityComparer<ITypeHierarchy>();

        [Obsolete("use alternative constructor")]
        public TypeHierarchy()
        {
            Implements = new HashSet<ITypeHierarchy>();
        }

        public TypeHierarchy(string elementQualifiedName) : this()
        {
            Element = TypeName.Get(elementQualifiedName);
        }

        public ITypeName Element { get; set; }
        public ITypeHierarchy Extends { get; set; }

        [NotNull]
        public ISet<ITypeHierarchy> Implements { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(TypeHierarchy other)
        {
            return Comparer.Equals(Implements, other.Implements) && Equals(Extends, other.Extends) && Equals(Element, other.Element);
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
            return string.Format("[Element: {0}, Extends: {1}, Implements: [{2}]]", Element, Extends, string.Join(", ", Implements));
        }
    }
}

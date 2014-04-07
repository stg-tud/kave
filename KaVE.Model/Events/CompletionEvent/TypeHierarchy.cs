using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;
using KaVE.Utils.Collections;

namespace KaVE.Model.Events.CompletionEvent
{
    [DataContract]
    public class TypeHierarchy : ITypeHierarchy
    {
        /// <summary>
        ///     For internal use only.
        /// </summary>
        [UsedImplicitly, Obsolete]
        public TypeHierarchy()
        {
            Implements = new HashSet<ITypeHierarchy>();
        }

        // ReSharper disable once CSharpWarnings::CS0612
        public TypeHierarchy(string elementQualifiedName) : this()
        {
            Element = TypeName.Get(elementQualifiedName);
        }

        [DataMember]
        public ITypeName Element { get; set; }

        [DataMember]
        public ITypeHierarchy Extends { get; set; }

        [DataMember]
        public ISet<ITypeHierarchy> Implements { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(TypeHierarchy other)
        {
            return Implements.SetEquals(other.Implements) && Equals(Extends, other.Extends) &&
                   Equals(Element, other.Element);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Implements.GetSetHashCode();
                hashCode = (hashCode*397) ^ (Extends != null ? Extends.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Element.GetHashCode());
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[Element: {0}, Extends: {1}, Implements: [{2}]]",
                Element,
                Extends,
                string.Join(", ", Implements));
        }

        public bool HasSupertypes
        {
            get { return HasSuperclass || IsImplementingInterfaces; }
        }

        public bool HasSuperclass
        {
            get { return Extends != null; }
        }

        public bool IsImplementingInterfaces
        {
            get { return Implements.Count > 0; }
        }
    }
}
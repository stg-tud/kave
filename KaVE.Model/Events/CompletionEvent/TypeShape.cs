using System.Collections.Generic;
using KaVE.JetBrains.Annotations;
using KaVE.Utils;
using KaVE.Utils.Collections;

namespace KaVE.Model.Events.CompletionEvent
{
    public class TypeShape
    {
        public TypeShape()
        {
            MethodHierarchies = new HashSet<MethodHierarchy>();
        }

        /// <summary>
        ///     A description of the enclosing class, including its parent class and implemented interfaces.
        /// </summary>
        public ITypeHierarchy TypeHierarchy { get; set; }

        /// <summary>
        ///     All Methods that are overridden in the class under edit (including information about the first and super
        ///     declaration).
        /// </summary>
        [NotNull]
        public ISet<MethodHierarchy> MethodHierarchies { get; set; }

        protected bool Equals(TypeShape other)
        {
            return Equals(TypeHierarchy, other.TypeHierarchy) &&
                   MethodHierarchies.SetEquals(other.MethodHierarchies);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TypeHierarchy != null ? TypeHierarchy.GetHashCode() : 0)*397) ^
                       MethodHierarchies.GetSetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[TypeHierarchy: {0}, MethodHierarchies: [{1}]]",
                TypeHierarchy,
                string.Join(", ", MethodHierarchies));
        }
    }
}
using System;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Groum;
using KaVE.Model.Names;
using KaVE.Utils;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    ///     The context of a code-completion event, i.e., a description of the code environment in which the completion
    ///     is triggered.
    /// </summary>
    public class Context
    {
        public Context()
        {
            CalledMethods = new HashSet<IMethodName>();
        }

        /// <summary>
        ///     Information about the method whose body is currently edited. <code>null</code> if completion is triggered
        ///     outside a method.
        /// </summary>
        // TODO replace hierarchy by method name, since hierarchy is contained in typeShape
        [Obsolete("use EnclosingMethod instead")]
        public MethodHierarchy EnclosingMethodHierarchy { get; set; }

        /// <summary>
        ///     Information about the method whose body is currently edited. <code>null</code> if completion is triggered
        ///     outside a method.
        /// </summary>
        public IMethodName EnclosingMethod
        {
            get { return EnclosingMethodHierarchy.Element; }
            set { EnclosingMethodHierarchy = new MethodHierarchy(value); }
        }

        /// <summary>
        ///     The GROUM derived from the current code in the enclosing method's body. This GROUM contains a completion
        ///     groum node that denotes the position code completion is triggered at.
        /// </summary>
        public IGroum EnclosingMethodGroum { get; set; }

        /// <summary>
        ///     Methods called in the EnclosingMethod. This is redundant information also contained in the EnclosingMethodGroum.
        /// </summary>
        [NotNull]
        public ISet<IMethodName> CalledMethods { get; set; }

        public TypeShape TypeShape { get; set; }


        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(Context other)
        {
            return Equals(EnclosingMethodHierarchy, other.EnclosingMethodHierarchy) &&
                   Equals(EnclosingMethodGroum, other.EnclosingMethodGroum) &&
                   CalledMethods.SetEquals(other.CalledMethods) && Equals(TypeShape, other.TypeShape);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (EnclosingMethodHierarchy != null ? EnclosingMethodHierarchy.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EnclosingMethodGroum != null ? EnclosingMethodGroum.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CalledMethods.GetHashCode();
                hashCode = (hashCode*397) ^ (TypeShape != null ? TypeShape.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "[EnclosingMethodHierarchy: {0}, EnclosingMethodGroum: {1}, CalledMethods: [{2}], TypeShape: {3}]",
                    EnclosingMethodHierarchy,
                    EnclosingMethodGroum,
                    string.Join(", ", CalledMethods),
                    TypeShape);
        }
    }
}
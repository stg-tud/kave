using System.Collections.Generic;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Groum;
using KaVE.Model.Names;
using KaVE.Utils;
using KaVE.Utils.Collections;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    /// The context of a code-completion event, i.e., a description of the code environment in which the completion
    /// is triggered.
    /// </summary>
    public class Context
    {
        public Context()
        {
            CalledMethods = new HashSet<IMethodName>();
            TypeShapeMethods = new HashSet<IMethodName>();
        }

        /// <summary>
        /// A description of the enclosing class, including its parent class and implemented interfaces.
        /// </summary>
        public ITypeHierarchy EnclosingClassHierarchy { get; set; }

        /// <summary>
        /// Information about the method whose body is currently edited. <code>null</code> if completion is triggered
        /// outside a method.
        /// </summary>
        public MethodDeclaration EnclosingMethodDeclaration { get; set; }

        /// <summary>
        /// The GROUM derived from the current code in the enclosing method's body. This GROUM contains a completion
        /// groum node that denotes the position code completion is triggered at.
        /// </summary>
        public IGroum EnclosingMethodGroum { get; set; }

        /// <summary>
        /// Methods called in the EnclosingMethod. This is redundant information also contained in the EnclosingMethodGroum.
        /// </summary>
        [NotNull]
        public ISet<IMethodName> CalledMethods { get; set; }

        /// <summary>
        /// All Methods that are
        /// </summary>
        [NotNull]
        public ISet<IMethodName> TypeShapeMethods { get; private set; }

        protected bool Equals(Context other)
        {
            return Equals(EnclosingClassHierarchy, other.EnclosingClassHierarchy) &&
                   Equals(EnclosingMethodDeclaration, other.EnclosingMethodDeclaration) &&
                   Equals(EnclosingMethodGroum, other.EnclosingMethodGroum) &&
                   CalledMethods.SetEquals(other.CalledMethods);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (EnclosingClassHierarchy != null ? EnclosingClassHierarchy.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EnclosingMethodDeclaration != null ? EnclosingMethodDeclaration.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EnclosingMethodGroum != null ? EnclosingMethodGroum.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CalledMethods.GetSetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "[EnclosingClassHierarchy: {0}, [EnclosingMethod: {1}], EnclosingMethodGroum: {2}, CalledMethods: {3}]",
                    EnclosingClassHierarchy,
                    EnclosingMethodDeclaration,
                    EnclosingMethodGroum,
                    CalledMethods);
        }
    }
}
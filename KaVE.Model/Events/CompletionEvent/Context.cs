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
        }

        /// <summary>
        /// A description of the enclosing class, including its parent class and implemented interfaces.
        /// </summary>
        public ITypeHierarchy EnclosingClassHierarchy { get; set; }

        /// <summary>
        /// The name of the method whose body is currently edited. <code>null</code> if completion is triggered
        /// outside a method.
        /// </summary>
        public IMethodName EnclosingMethod { get; set; }

        /// <summary>
        /// The implementation of the enclosing method that is referred to by calling
        /// <code>super.'methodName'(...)</code>.
        /// </summary>
        public IMethodName EnclosingMethodSuper { get; set; }

        /// <summary>
        /// The declarations of the enclosing method, i.e., the method names specified in interfaces or the highest
        /// parent class that the enclosing method is an implementation of.
        /// </summary>
        public IMethodName EnclosingMethodFirst { get; set; }

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

        // TODO add information about other methods, to include class path information?

        protected bool Equals(Context other)
        {
            return Equals(EnclosingClassHierarchy, other.EnclosingClassHierarchy) &&
                   Equals(EnclosingMethod, other.EnclosingMethod) &&
                   Equals(EnclosingMethodSuper, other.EnclosingMethodSuper) &&
                   Equals(EnclosingMethodFirst, other.EnclosingMethodFirst) &&
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
                hashCode = (hashCode*397) ^ (EnclosingMethod != null ? EnclosingMethod.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EnclosingMethodSuper != null ? EnclosingMethodSuper.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EnclosingMethodFirst != null ? EnclosingMethodFirst.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EnclosingMethodGroum != null ? EnclosingMethodGroum.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CalledMethods.GetSetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "[EnclosingClassHierarchy: {0}, [EnclosingMethod: {1}, Super: {2}, First: {3}], EnclosingMethodGroum: {4}, CalledMethods: {5}]",
                    EnclosingClassHierarchy,
                    EnclosingMethod,
                    EnclosingMethodSuper,
                    EnclosingMethodFirst,
                    EnclosingMethodGroum,
                    CalledMethods);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Groum;
using KaVE.Model.Names;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    /// The context of a code-completion event, i.e., a description of the code environment in which the completion
    /// is triggered.
    /// </summary>
    public class Context
    {
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
        public ISet<IMethodName> EnclosingMethodFirst { get; set; }

        /// <summary>
        /// The GROUM derived from the current code in the enclosing method's body. This GROUM contains a completion
        /// groum node that denotes the position code completion is triggered at.
        /// </summary>
        public IGroum EnclosingMethodGroum { get; set; }

        // TODO add information about other methods, to include class path information?

        private bool Equals(Context other)
        {
            return Equals(EnclosingClassHierarchy, other.EnclosingClassHierarchy)
                && Equals(EnclosingMethod, other.EnclosingMethod)
                && Equals(EnclosingMethodSuper, other.EnclosingMethodSuper)
                && EnclosingMethodFirst != null && EnclosingMethodFirst.SequenceEqual(other.EnclosingMethodFirst)
                && Equals(EnclosingMethodGroum, other.EnclosingMethodGroum);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Context) obj);
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
                return hashCode;
            }
        }

        public override string ToString()
        {
            return EnclosingClassHierarchy + " {\n" + EnclosingMethod + " {\n" + EnclosingMethodGroum + "\n}\n}";
        }
    }
}
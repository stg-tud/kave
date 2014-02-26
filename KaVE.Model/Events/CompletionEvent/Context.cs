using System.Collections.Generic;
using System.Linq;
using KaVE.JetBrains.Annotations;
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
        public Context()
        {
            EnclosingMethodFirst = new HashSet<IMethodName>();
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
        [NotNull]
        public ISet<IMethodName> EnclosingMethodFirst { get; set; }

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


    }
}
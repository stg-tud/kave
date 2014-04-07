using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
    [DataContract]
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
        // TODO replace hierarchy by method name, since hierarchy is contained in typeShape. remember to regenerate equals+hashcode
        [Obsolete("use EnclosingMethod instead")]
        public MethodHierarchy EnclosingMethodHierarchy { get; set; }

        /// <summary>
        ///     Information about the method whose body is currently edited. <code>null</code> if completion is triggered
        ///     outside a method.
        /// </summary>
        [DataMember]
        public IMethodName EnclosingMethod
        {
            // ReSharper disable CSharpWarnings::CS0618
            get { return EnclosingMethodHierarchy != null ? EnclosingMethodHierarchy.Element : null; }
            set { EnclosingMethodHierarchy = value != null ? new MethodHierarchy(value) : null; }
            // ReSharper restore CSharpWarnings::CS0618
        }

        /// <summary>
        ///     The GROUM derived from the current code in the enclosing method's body. This GROUM contains a completion
        ///     groum node that denotes the position code completion is triggered at.
        /// </summary>
        public GroumBase EnclosingMethodGroum { get; set; }

        /// <summary>
        ///     Methods called in the EnclosingMethod. This is redundant information also contained in the EnclosingMethodGroum.
        /// </summary>
        [NotNull, DataMember]
        public ISet<IMethodName> CalledMethods { get; set; }

        [DataMember]
        public TypeShape TypeShape { get; set; }

        /// <summary>
        ///     The type of the reference completion was triggered on or <code>null</code>, if completion was triggered without an
        ///     (explicit) reference.
        /// </summary>
        [DataMember]
        public IName TriggerTarget { get; set; }


        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(Context other)
        {
            // ReSharper disable CSharpWarnings::CS0618
            return Equals(EnclosingMethodHierarchy, other.EnclosingMethodHierarchy) &&
                   Equals(EnclosingMethodGroum, other.EnclosingMethodGroum) &&
                   CalledMethods.SetEquals(other.CalledMethods) && Equals(TypeShape, other.TypeShape);
            // ReSharper restore CSharpWarnings::CS0618
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable CSharpWarnings::CS0618
                var hashCode = (EnclosingMethodHierarchy != null ? EnclosingMethodHierarchy.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EnclosingMethodGroum != null ? EnclosingMethodGroum.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CalledMethods.GetHashCode();
                hashCode = (hashCode*397) ^ (TypeShape != null ? TypeShape.GetHashCode() : 0);
                return hashCode;
                // ReSharper restore CSharpWarnings::CS0618
            }
        }

        public override string ToString()
        {
            // ReSharper disable CSharpWarnings::CS0618
            return
                string.Format(
                    "[EnclosingMethodHierarchy: {0}, EnclosingMethodGroum: {1}, CalledMethods: [{2}], TypeShape: {3}]",
                    EnclosingMethodHierarchy,
                    EnclosingMethodGroum,
                    string.Join(", ", CalledMethods),
                    TypeShape);
            // ReSharper restore CSharpWarnings::CS0618
        }
    }
}
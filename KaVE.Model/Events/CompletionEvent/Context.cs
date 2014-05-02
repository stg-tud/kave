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
        [Obsolete("use EnclosingMethod instead")]
        public MethodHierarchy EnclosingMethodHierarchy
        {
            get { throw new NotImplementedException(); }
            set { EnclosingMethod = value != null ? value.Element : null; }
        }

        /// <summary>
        ///     Information about the method whose body is currently edited. <code>null</code> if completion is triggered
        ///     outside a method.
        /// </summary>
        [DataMember]
        public IMethodName EnclosingMethod { get; set; }

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
            return Equals(EnclosingMethod, other.EnclosingMethod) &&
                   Equals(EnclosingMethodGroum, other.EnclosingMethodGroum) &&
                   CalledMethods.SetEquals(other.CalledMethods) && Equals(TypeShape, other.TypeShape);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (EnclosingMethod != null ? EnclosingMethod.GetHashCode() : 0);
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
                    "[EnclosingMethod: {0}, EnclosingMethodGroum: {1}, CalledMethods: [{2}], TypeShape: {3}]",
                    EnclosingMethod,
                    EnclosingMethodGroum,
                    string.Join(", ", CalledMethods),
                    TypeShape);
        }
    }
}
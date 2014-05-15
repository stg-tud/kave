/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
            EntryPointsToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>();
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
        /// Maps from entry points to the methods called in the call-graph below the respective entry point.
        /// </summary>
        [NotNull, DataMember]
        public IDictionary<IMethodName, ISet<IMethodName>> EntryPointsToCalledMethods;

        public ICollection<IMethodName> EntryPoints
        {
            get { return EntryPointsToCalledMethods.Keys; }
        }

        // TODO @Sven: remove this property
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
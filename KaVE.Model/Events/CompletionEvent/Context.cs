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
 * 
 * Contributors:
 *    - Sven Amann
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using KaVE.JetBrains.Annotations;
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
            EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>();
            EntryPointToGroum = new Dictionary<IMethodName, Groums.Groum>();
        }

        /// <summary>
        ///     Information about the method whose body is currently edited. <code>null</code> if completion is triggered
        ///     outside a method.
        /// </summary>
        [DataMember]
        public IMethodName EnclosingMethod { get; set; }

        /// <summary>
        ///     Maps from entry points to the derived GROUM of those methods.
        /// </summary>
        public IDictionary<IMethodName, Groums.Groum> EntryPointToGroum { get; set; }

        /// <summary>
        ///     Maps from entry points to the methods called in the call-graph below the respective entry point.
        /// </summary>
        [NotNull, DataMember]
        public IDictionary<IMethodName, ISet<IMethodName>> EntryPointToCalledMethods { get; set; }

        public ICollection<IMethodName> EntryPoints
        {
            get { return EntryPointToCalledMethods.Keys; }
        }

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
            return EntryPointToCalledMethods.DeepEquals(other.EntryPointToCalledMethods) &&
                   Equals(EnclosingMethod, other.EnclosingMethod) &&
                   EqualityUtils.Equals(EntryPointToGroum, other.EntryPointToGroum) &&
                   Equals(TypeShape, other.TypeShape) &&
                   Equals(TriggerTarget, other.TriggerTarget);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 397;
                hashCode = (hashCode*397) ^ (EnclosingMethod != null ? EnclosingMethod.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (TypeShape != null ? TypeShape.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TriggerTarget != null ? TriggerTarget.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ HashCodeUtils.For(191, EntryPointToCalledMethods);
                hashCode = (hashCode * 397) ^ HashCodeUtils.For(193, EntryPointToGroum);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "[EnclosingMethod: {0}, Groums: {1}, CalledMethods: [{2}], TypeShape: {3}]",
                    EnclosingMethod,
                    ToString(EntryPointToGroum),
                    ToString(EntryPointToCalledMethods),
                    TypeShape);
        }

        private string ToString(IEnumerable<KeyValuePair<IMethodName, ISet<IMethodName>>> dictionary)
        {
            var builder = new StringBuilder();
            foreach (var keyValuePair in dictionary)
            {
                builder.Append(keyValuePair.Key);
                builder.Append(":{");
                builder.Append(string.Join(",", keyValuePair.Value));
                builder.Append("},");
            }
            return builder.ToString();
        }

        private string ToString(IEnumerable<KeyValuePair<IMethodName, Groums.Groum>> dictionary)
        {
            var builder = new StringBuilder();
            foreach (var keyValuePair in dictionary)
            {
                builder.Append(keyValuePair.Key);
                builder.Append(":");
                builder.Append(keyValuePair.Value);
            }
            return builder.ToString();
        }
    }
}
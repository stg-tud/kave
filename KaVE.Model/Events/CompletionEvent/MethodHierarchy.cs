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
 *    - Sebastian Proksch
 *    - Sven Amann
 */

using System.Runtime.Serialization;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Utils;

namespace KaVE.Model.Events.CompletionEvent
{
    [DataContract]
    public class MethodHierarchy
    {
        /// <summary>
        ///     The name of a method.
        /// </summary>
        [NotNull, DataMember]
        public IMethodName Element { get; private set; }

        /// <summary>
        ///     The implementation of the enclosing method that is referred to by calling
        ///     <code>super.'methodName'(...)</code>.
        /// </summary>
        [DataMember]
        public IMethodName Super { get; set; }

        /// <summary>
        ///     The declarations of the enclosing method, i.e., the method names specified in interfaces or the highest
        ///     parent class that the enclosing method is an implementation of.
        /// </summary>
        [DataMember]
        public IMethodName First { get; set; }

        /// <summary>
        ///     Wheather or not this is a hierarchy of a method that overrides or implements a declaration from further up in the
        ///     type hierarchy.
        /// </summary>
        public bool IsOverrideOrImplementation
        {
            get { return First != null; }
        }

        public MethodHierarchy([NotNull] IMethodName methodName)
        {
            Element = methodName;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(MethodHierarchy other)
        {
            return Element.Equals(other.Element) && Equals(Super, other.Super) && Equals(First, other.First);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Element.GetHashCode();
                hashCode = (hashCode*397) ^ (Super != null ? Super.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (First != null ? First.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("[Element: {0}, Super: {1}, First: {2}]", Element, Super, First);
        }
    }
}
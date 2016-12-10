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

using System.Runtime.Serialization;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.TypeShapes
{
    public interface IMemberHierarchy<TMember> where TMember : IMemberName
    {
        /// <summary>
        ///     The name of the member.
        /// </summary>
        [NotNull]
        TMember Element { get; set; }

        /// <summary>
        ///     The implementation of the enclosing method that is referred to by calling
        ///     <code>base.'memberName'(...)</code>.
        /// </summary>
        [CanBeNull]
        TMember Super { get; set; }

        /// <summary>
        ///     The definition that introduces the member, i.e., interfaces or the
        ///     parent class "highest" in the hiearchy that has it.
        /// </summary>
        [CanBeNull]
        TMember First { get; set; }

        /// <summary>
        ///     Whether or not this is a hierarchy of a member that overrides or implements a declaration from further up in the
        ///     type hierarchy.
        /// </summary>
        /// // TODO (seb): I'm not sure this definition makes sense right now. Analyze usages and rename.
        bool IsDeclaredInParentHierarchy { get; }
    }

    [DataContract]
    public abstract class BaseMemberHierarchy<TMember> : IMemberHierarchy<TMember> where TMember : IMemberName
    {
        [DataMember]
        public TMember Element { get; set; }

        [DataMember]
        public TMember Super { get; set; }

        [DataMember]
        public TMember First { get; set; }

        public bool IsDeclaredInParentHierarchy
        {
            get { return First != null; }
        }

        protected BaseMemberHierarchy([NotNull] TMember elem)
        {
            Element = elem;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(BaseMemberHierarchy<TMember> other)
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
            return this.ToStringReflection();
        }
    }
}
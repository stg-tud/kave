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
using System.Collections.Generic;
using System.Runtime.Serialization;
using KaVE.JetBrains.Annotations;
using KaVE.Utils;
using KaVE.Utils.Collections;

namespace KaVE.Model.Events.CompletionEvent
{
    [DataContract]
    public class TypeShape
    {
        public TypeShape()
        {
            MethodHierarchies = new HashSet<MethodHierarchy>();
        }

        /// <summary>
        ///     A description of the enclosing class, including its parent class and implemented interfaces.
        /// </summary>
        [DataMember]
        public ITypeHierarchy TypeHierarchy { get; set; }

        /// <summary>
        ///     All Methods that are overridden in the class under edit (including information about the first and super
        ///     declaration).
        /// </summary>
        [NotNull, DataMember]
        public ISet<MethodHierarchy> MethodHierarchies { get; set; }

        protected bool Equals(TypeShape other)
        {
            return Equals(TypeHierarchy, other.TypeHierarchy) &&
                   MethodHierarchies.SetEquals(other.MethodHierarchies);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TypeHierarchy != null ? TypeHierarchy.GetHashCode() : 0)*397) ^
                       MethodHierarchies.GetSetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[TypeHierarchy: {0}, MethodHierarchies: [{1}]]",
                TypeHierarchy,
                string.Join(", ", MethodHierarchies));
        }
    }
}
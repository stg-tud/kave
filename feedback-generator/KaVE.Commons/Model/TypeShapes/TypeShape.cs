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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.TypeShapes
{
    [DataContract]
    public class TypeShape : ITypeShape
    {
        [DataMember]
        public ITypeHierarchy TypeHierarchy { get; set; }

        [DataMember]
        public IKaVESet<IMethodHierarchy> MethodHierarchies { get; set; }

        public TypeShape()
        {
            TypeHierarchy = new TypeHierarchy();
            MethodHierarchies = Sets.NewHashSet<IMethodHierarchy>();
        }

        private bool Equals(TypeShape other)
        {
            return Equals(TypeHierarchy, other.TypeHierarchy) &&
                   Equals(MethodHierarchies, other.MethodHierarchies);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TypeHierarchy.GetHashCode()*397) ^
                       MethodHierarchies.GetSetHashCode();
            }
        }
    }
}
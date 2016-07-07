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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.TypeShapes
{
    [DataContract]
    public class TypeHierarchy : ITypeHierarchy
    {
        [DataMember]
        public ITypeName Element { get; set; }

        [DataMember]
        public ITypeHierarchy Extends { get; set; }

        [DataMember]
        public IKaVESet<ITypeHierarchy> Implements { get; set; }

        public bool HasSupertypes
        {
            get { return HasSuperclass || IsImplementingInterfaces; }
        }

        public bool HasSuperclass
        {
            get { return Extends != null; }
        }

        public bool IsImplementingInterfaces
        {
            get { return Implements.Count > 0; }
        }

        public TypeHierarchy()
        {
            Element = Names.UnknownType;
            Implements = Sets.NewHashSet<ITypeHierarchy>();
        }

        public TypeHierarchy(string elementQualifiedName)
            : this()
        {
            Element = Names.Type(elementQualifiedName);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(TypeHierarchy other)
        {
            return Equals(Implements, other.Implements) && Equals(Extends, other.Extends) &&
                   Equals(Element, other.Element);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Implements.GetSetHashCode();
                hashCode = (hashCode*397) ^ (Extends != null ? Extends.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Element.GetHashCode());
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
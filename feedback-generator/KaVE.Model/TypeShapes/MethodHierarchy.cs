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
using KaVE.Model.Names.CSharp;
using KaVE.Utils;

namespace KaVE.Model.TypeShapes
{
    [DataContract]
    public class MethodHierarchy : IMethodHierarchy
    {
        [DataMember]
        public IMethodName Element { get; set; }

        [DataMember]
        public IMethodName Super { get; set; }

        [DataMember]
        public IMethodName First { get; set; }

        public bool IsDeclaredInParentHierarchy
        {
            get { return First != null; }
        }

        public MethodHierarchy()
        {
            Element = MethodName.UnknownName;
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
    }
}
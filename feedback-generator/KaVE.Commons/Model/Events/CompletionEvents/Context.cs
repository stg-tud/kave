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
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.CompletionEvents
{
    /// <summary>
    ///     The context of a code-completion event, i.e., a description of the code environment in which the completion
    ///     is triggered.
    /// </summary>
    [DataContract]
    public class Context
    {
        [DataMember, NotNull]
        public ITypeShape TypeShape { get; set; }

        [DataMember, NotNull]
        public ISST SST { get; set; }

        [NotNull]
        public static Context Default
        {
            get { return new Context(); }
        }

        public Context()
        {
            TypeShape = new TypeShape();
            SST = new SST();
        }

        public bool IsDefault()
        {
            return Equals(Default);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(Context other)
        {
            return Equals(TypeShape, other.TypeShape) &&
                   Equals(SST, other.SST);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 397;
                hashCode = (hashCode*397) ^ TypeShape.GetHashCode();
                hashCode = (hashCode*397) ^ SST.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
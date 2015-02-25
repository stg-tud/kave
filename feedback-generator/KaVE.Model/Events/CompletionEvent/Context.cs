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

using System.Runtime.Serialization;
using KaVE.JetBrains.Annotations;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl;
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
        public static Context Empty
        {
            get { return new Context(); }
        }

        public Context()
        {
            TypeShape = new TypeShape();
            SST = new SST();
        }

        [DataMember, NotNull]
        public TypeShape TypeShape { get; set; }

        [DataMember, NotNull]
        public SST SST { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(Context other)
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
            return string.Format("[TypeShape: {0}, SST: {1}]", TypeShape, SST);
        }
    }
}
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
using System.Linq;
using System.Runtime.Serialization;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Events.VisualStudio
{
    [DataContract]
    public class IDEStateEvent : IDEEvent
    {
        public IDEStateEvent()
        {
            OpenWindows = new List<IWindowName>();
            OpenDocuments = new List<IDocumentName>();
        }

        [DataMember]
        public IDELifecyclePhase IDELifecyclePhase { get; set; }

        [DataMember]
        public IList<IWindowName> OpenWindows { get; set; }

        [DataMember]
        public IList<IDocumentName> OpenDocuments { get; set; }

        protected bool Equals(IDEStateEvent other)
        {
            return base.Equals(other) && IDELifecyclePhase == other.IDELifecyclePhase &&
                   OpenWindows.SequenceEqual(other.OpenWindows) && OpenDocuments.SequenceEqual(other.OpenDocuments);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (int) IDELifecyclePhase;
                hashCode = (hashCode*397) ^ (OpenWindows != null ? HashCodeUtils.For(397, OpenWindows) : 0);
                hashCode = (hashCode*397) ^ (OpenDocuments != null ? HashCodeUtils.For(397, OpenDocuments) : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }

    public enum IDELifecyclePhase
    {
        /// <summary>
        ///     State snapshot is taken immediately after startup, i.e., before first user interactions.
        /// </summary>
        Startup,

        /// <summary>
        ///     State snapshot is taken before shutdown, i.e., after the last user interaction.
        /// </summary>
        Shutdown,

        /// <summary>
        ///     State snapshot is taken somewhen during runtime.
        /// </summary>
        Runtime
    }
}
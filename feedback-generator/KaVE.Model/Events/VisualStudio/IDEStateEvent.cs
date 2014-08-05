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
using KaVE.Model.Names.VisualStudio;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class IDEStateEvent : IDEEvent
    {
        public enum LifecyclePhase
        {
            /// <summary>
            /// State snapshot is taken immediately after startup, i.e., before first user interactions.
            /// </summary>
            Startup,

            /// <summary>
            /// State snapshot is taken before shutdown, i.e., after the last user interaction.
            /// </summary>
            Shutdown,

            /// <summary>
            /// State snapshot is taken somewhen during runtime.
            /// </summary>
            Runtime
        }

        [DataMember]
        public LifecyclePhase IDELifecyclePhase { get; set; }

        [DataMember]
        public IList<WindowName> OpenWindows { get; set; }

        [DataMember]
        public IList<DocumentName> OpenDocuments { get; set; }
    }
}
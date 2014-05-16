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
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class BuildEvent : IDEEvent
    {
        public BuildEvent()
        {
            Targets = new List<BuildTarget>();
        }

        [DataMember]
        public string Scope { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public IList<BuildTarget> Targets { get; set; }
    }

    [DataContract]
    public class BuildTarget
    {
        [DataMember]
        public string Project { get; set; }
        [DataMember]
        public string ProjectConfiguration { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public string SolutionConfiguration { get; set; }
        [DataMember]
        public DateTime? StartedAt { get; set; }
        [DataMember]
        public TimeSpan? Duration { get; set; }
        [DataMember]
        public bool Successful { get; set; }
    }
}

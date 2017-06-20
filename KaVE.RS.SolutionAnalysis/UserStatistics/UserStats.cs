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
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Collections;

namespace KaVE.RS.SolutionAnalysis.UserStatistics
{
    public class UserStats
    {
        public int NumEvents { get; set; }
        public int NumComplEvents { get; set; }
        public DateTime FirstEvent { get; set; }
        public DateTime LastEvent { get; set; }
        public IKaVESet<string> ParticipatedDays { get; set; }
        public IKaVESet<string> Ids { get; set; }
        public  IKaVEList<IUserProfileEvent> Profiles { get; set; }
        public Positions Position { get; set; }

        public UserStats()
        {
           /* ParticipatedDays = Sets.NewHashSet<string>();
            Ids = Sets.NewHashSet<string>();
            Profiles = Lists.NewList<IUserProfileEvent>();*/
        }
    }
}
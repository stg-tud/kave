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
using System.Linq;
using Newtonsoft.Json;

namespace KaVE.VS.Achievements.BaseClasses.AchievementTypes
{
    /// <summary>
    ///     Base class for modelling an achievement that has more than one stage of progress
    ///     (e.g. open a file 10 times, open a file 100 times, open a file 1000 times, ...)
    /// </summary>
    public class StagedAchievement : BaseAchievement
    {
        public int[] Ids;

        public StagedAchievement(int[] ids) : base(ids[0])
        {
            Ids = ids;
            Stages = new ProgressAchievement[Ids.Count()];
            Initialize();
            CompletedEventHandler += SetNextStage;
        }

        /// <summary>
        ///     The Current Stage of this Achievement;
        ///     Can't be less than zero and more than the length of <see cref="Stages" /> minus one
        /// </summary>
        public int CurrentStage { get; set; }

        public ProgressAchievement[] Stages { get; set; }

        [JsonIgnore]
        public ProgressAchievement CurrentStageAchievement
        {
            get { return Stages[CurrentStage]; }
        }

        [JsonIgnore]
        public ProgressAchievement FirstStageAchievement
        {
            get { return Stages[0]; }
        }

        [JsonIgnore]
        public ProgressAchievement LastStageAchievement
        {
            get { return Stages[Stages.Length - 1]; }
        }

        [JsonIgnore]
        public ProgressAchievement HighestCompletedStageOrFirst
        {
            get
            {
                if (IsCompleted)
                {
                    return LastStageAchievement;
                }
                return CurrentStage > 0 ? Stages[CurrentStage - 1] : FirstStageAchievement;
            }
        }

        /// <summary>
        ///     Resets the Achievement and its <see cref="Stages" />
        /// </summary>
        public new void Initialize()
        {
            CurrentStage = 0;
            for (var i = 0; i < Ids.Count(); i++)
            {
                Stages[i] = new ProgressAchievement(Ids[i]);
            }
            base.Initialize();
        }

        /// <summary>
        ///     Must not be called manually
        /// </summary>
        public void SetNextStage(object o, EventArgs args)
        {
            var completedAchievement = o as ProgressAchievement;
            if (!CurrentStageAchievement.Equals(completedAchievement))
            {
                return;
            }

            if (completedAchievement == LastStageAchievement)
            {
                Unlock();
            }
            else
            {
                CurrentStage++;
            }
        }
    }
}
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
using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Achievements.Listing;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;

namespace KaVE.VS.Achievements.Achievements.BaseClasses.CalculatorTypes
{
    /// <summary>
    ///     Abstract Class for implementing Calculators for Staged Achievements that use T as progress values
    /// </summary>
    public abstract class StagedAchievementCalculator<T> : AchievementCalculator
    {
        protected StagedAchievementCalculator(StagedAchievement achievement,
            IAchievementListing achievementListing,
            IStatisticListing statisticListing,
            IObservable<IStatistic> observable)
            : base(achievement, achievementListing, statisticListing, observable) {}

        private StagedAchievement StagedAchievement
        {
            get { return Achievement as StagedAchievement; }
        }

        /// <summary>
        ///     Calculates the Stages of the StagedAchievement using the given <paramref name="currentValue" />
        /// </summary>
        protected void StagedCalculation(T currentValue)
        {
            foreach (var stage in StagedAchievement.Stages)
            {
                stage.CurrentValue = currentValue;
                stage.CurrentProgress = CalculateProgress(currentValue, stage);
                if (stage.CurrentProgress >= 100)
                {
                    stage.Unlock();
                }
            }
        }

        protected abstract double CalculateProgress(T currentValue, ProgressAchievement targetAchievement);
    }
}
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
    ///     Abstract Class for implementing Calculators for Staged Achievements that use integers as progress values;
    ///     Target Values of this Achievements Stages must be integers!
    /// </summary>
    public abstract class IntegerStagedAchievementCalculator : StagedAchievementCalculator<int>
    {
        protected IntegerStagedAchievementCalculator(StagedAchievement achievement,
            IAchievementListing achievementListing,
            IStatisticListing statisticListing,
            IObservable<IStatistic> observable)
            : base(achievement, achievementListing, statisticListing, observable) {}

        protected override double CalculateProgress(int value, ProgressAchievement targetAchievement)
        {
            try
            {
                return 100*(double) value/(int) targetAchievement.TargetValue;
            }
            catch
            {
                Unsubscriber.Dispose();
                throw new InvalidCastException(
                    "Target Value is not an integer; Inherit from StagedAchievementCalculator for other target value types.");
            }
        }
    }
}
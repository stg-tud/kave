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
using JetBrains.Application;
using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Achievements.BaseClasses.CalculatorTypes;
using KaVE.VS.Achievements.Achievements.Listing;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;

namespace KaVE.VS.Achievements.Achievements.Calculators
{
    [ShellComponent]
    public class TimeInDebugCalculator : StagedAchievementCalculator<TimeSpan>
    {
        public static readonly int[] Ids =
        {
            35,
            36,
            37,
            38,
            39
        };

        public TimeInDebugCalculator(IAchievementListing achievementListing,
            IStatisticListing statisticListing,
            IObservable<IStatistic> observable)
            : base(new StagedAchievement(Ids), achievementListing, statisticListing, observable) {}

        protected override bool Calculate(IStatistic statistic)
        {
            var globalStatistic = statistic as GlobalStatistic;
            if (globalStatistic == null)
            {
                return false;
            }

            StagedCalculation(globalStatistic.TimeInDebugSession);
            return true;
        }

        protected override double CalculateProgress(TimeSpan currentValue, ProgressAchievement targetAchievement)
        {
            var currentMinutes = currentValue.TotalMinutes;
            var targetMinutes = ((TimeSpan) targetAchievement.TargetValue).TotalMinutes;
            return currentMinutes/targetMinutes*100;
        }
    }
}
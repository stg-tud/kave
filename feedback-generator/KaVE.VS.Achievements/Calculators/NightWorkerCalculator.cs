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
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.BaseClasses.CalculatorTypes;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;

namespace KaVE.VS.Achievements.Calculators
{
    [ShellComponent]
    public class NightWorkerCalculator : AchievementCalculator
    {
        public const int Id = 15;

        public NightWorkerCalculator(IAchievementListing achievementListing,
            IStatisticListing statisticListing,
            IObservable<IStatistic> observable)
            : base(new BaseAchievement(Id), achievementListing, statisticListing, observable) {}

        protected override bool Calculate(IStatistic statistic)
        {
            var globalStatistic = statistic as GlobalStatistic;
            if (globalStatistic == null)
            {
                return false;
            }

            if ((globalStatistic.EarliestEventTime != null &&
                 globalStatistic.EarliestEventTime.Value.TimeOfDay.TotalHours > 4) ||
                (globalStatistic.LatestEventTime != null &&
                 globalStatistic.LatestEventTime.Value.TimeOfDay.TotalHours < 22))
            {
                return false;
            }

            Achievement.Unlock();
            return true;
        }
    }
}
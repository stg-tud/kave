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
using JetBrains.Application;
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.BaseClasses.CalculatorTypes;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;

namespace KaVE.VS.Achievements.Calculators
{
    [ShellComponent]
    public class TotalCommitsCalculator : IntegerStagedAchievementCalculator
    {
        public static readonly int[] Ids =
        {
            30,
            31,
            32,
            33,
            34
        };

        public TotalCommitsCalculator(IAchievementListing achievementListing,
            IStatisticListing statisticListing,
            IObservable<IStatistic> observable)
            : base(new StagedAchievement(Ids), achievementListing, statisticListing, observable) {}

        protected override bool Calculate(IStatistic statistic)
        {
            var commandStatistic = statistic as CommandStatistic;
            if (commandStatistic == null)
            {
                return false;
            }

            var totalCommitsKey =
                commandStatistic.CommandTypeValues.Keys.FirstOrDefault(key => key.Contains("Team.Git.Commit"));

            var totalCommitsAndPushKey =
                commandStatistic.CommandTypeValues.Keys.FirstOrDefault(key => key.Contains("Team.Git.CommitAndPush"));

            var totalCommitsAndSyncKey =
                commandStatistic.CommandTypeValues.Keys.FirstOrDefault(key => key.Contains("Team.Git.CommitAndSync"));

            var totalClickedCommitsKey =
                commandStatistic.CommandTypeValues.Keys.FirstOrDefault(key => key.Contains("Comm_it"));

            var totalCommitsValue = 0;

            if (totalCommitsKey != default(string) && !totalCommitsKey.Contains("Team.Git.CommitAndPush") &&
                !totalCommitsKey.Contains("Team.Git.CommitAndSync"))
            {
                totalCommitsValue += commandStatistic.CommandTypeValues[totalCommitsKey];
            }

            if (totalCommitsAndPushKey != default(string))
            {
                totalCommitsValue += commandStatistic.CommandTypeValues[totalCommitsAndPushKey];
            }

            if (totalCommitsAndSyncKey != default(string))
            {
                totalCommitsValue += commandStatistic.CommandTypeValues[totalCommitsAndSyncKey];
            }

            if (totalClickedCommitsKey != default(string))
            {
                totalCommitsValue += commandStatistic.CommandTypeValues[totalClickedCommitsKey];
            }

            StagedCalculation(totalCommitsValue);
            return true;
        }
    }
}
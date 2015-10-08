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
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Statistics;
using KaVE.VS.Statistics.Statistics;

namespace KaVE.VS.Achievements.BaseClasses.CalculatorTypes
{
    /// <summary>
    ///     Abstract Class for implementing Calculators for Base Achievements
    /// </summary>
    public abstract class AchievementCalculator : IObserver<IStatistic>, IAchievementCalculator
    {
        /// <summary>
        ///     Gets updated if the achievement changes;
        ///     Persists <see cref="Achievement" /> between sessions
        /// </summary>
        protected readonly IAchievementListing AchievementListing;

        protected readonly IObservable<IStatistic> Observable;
        protected readonly IStatisticListing StatisticListing;

        protected BaseAchievement Achievement;
        protected IDisposable Unsubscriber;

        /// <summary>
        ///     Implicitly updates the <see cref="AchievementListing" /> with <see cref="Achievement" />;
        ///     Subscribes to <paramref name="observable" />
        /// </summary>
        /// <param name="achievement">
        ///     Used for <see cref="Achievement" /> if it is not already listed in
        ///     <see cref="AchievementListing" />
        /// </param>
        /// <param name="achievementListing">Sets <see cref="AchievementListing" /></param>
        /// <param name="statisticListing">Sets <see cref="StatisticListing" /></param>
        /// <param name="observable">Sets <see cref="Observable" /></param>
        protected AchievementCalculator(BaseAchievement achievement,
            IAchievementListing achievementListing,
            IStatisticListing statisticListing,
            IObservable<IStatistic> observable)
        {
            AchievementListing = achievementListing;
            StatisticListing = statisticListing;
            Observable = observable;

            Unsubscriber = observable.Subscribe(this);

            Achievement = AchievementListing.GetAchievement(achievement.Id) ?? achievement;

            AchievementListing.Update(Achievement);
        }

        /// <summary>
        ///     Resets <see cref="Achievement" /> and updates <see cref="AchievementListing" /> with the initialized Achievement;
        ///     Resubscribes to <see cref="Observable" />
        /// </summary>
        public virtual void ResetAchievement()
        {
            Achievement.Initialize();

            var progressAchievement = Achievement as ProgressAchievement;
            if (progressAchievement != null)
            {
                progressAchievement.Initialize();
            }

            var stagedAchievement = Achievement as StagedAchievement;
            if (stagedAchievement != null)
            {
                stagedAchievement.Initialize();
            }

            Unsubscriber = Observable.Subscribe(this);
            AchievementListing.Update(Achievement);
        }

        /// <summary>
        ///     Calculates the new Achievement from <paramref name="statistic" />;
        ///     Unsubscribes from further updates if <see cref="Achievement" /> is completed
        /// </summary>
        public void OnNext(IStatistic statistic)
        {
            var achievementWasUpdated = Calculate(statistic);

            if (Achievement.IsCompleted)
            {
                Unsubscriber.Dispose();
            }
            if (achievementWasUpdated)
            {
                AchievementListing.Update(Achievement);
            }
        }

        /// <summary>
        ///     Does nothing;
        ///     Must be implemented bause of the IObserver Interface
        /// </summary>
        public void OnError(Exception error) {}

        /// <summary>
        ///     Does nothing;
        ///     Must be implemented bause of the IObserver Interface
        /// </summary>
        public void OnCompleted() {}

        /// <summary>
        ///     This method implements the update logic for the Achievement
        /// </summary>
        /// <param name="statistic">The updated statistic</param>
        /// <returns>If the Achievement was changed</returns>
        protected abstract bool Calculate(IStatistic statistic);
    }
}
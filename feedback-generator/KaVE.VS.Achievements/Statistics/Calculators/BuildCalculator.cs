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

using System.Linq;
using JetBrains.Application;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.VS.Achievements.Statistics.Calculators.BaseClasses;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.Achievements.Statistics.Calculators
{
    [ShellComponent]
    public class BuildCalculator : StatisticCalculator
    {
        public BuildCalculator(IStatisticListing statisticListing, IMessageBus messageBus, IErrorHandler errorHandler)
            : base(statisticListing, messageBus, errorHandler, typeof (BuildStatistic)) {}

        protected override IStatistic Process(IDEEvent @event)
        {
            var buildEvent = @event as BuildEvent;
            if (buildEvent == null)
            {
                return null;
            }

            var buildStatistic = (BuildStatistic) StatisticListing.GetStatistic(StatisticType);

            var buildSuccessful = buildEvent.Targets.Where(buildTarget => !buildTarget.Successful).IsEmpty();

            if (buildSuccessful)
            {
                buildStatistic.SuccessfulBuilds++;
            }
            else
            {
                buildStatistic.FailedBuilds++;
            }

            return buildStatistic;
        }
    }
}
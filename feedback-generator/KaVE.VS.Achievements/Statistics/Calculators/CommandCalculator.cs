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

using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.VS.Achievements.Statistics.Calculators.BaseClasses;
using KaVE.VS.Achievements.Statistics.Filters;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.Achievements.Statistics.Calculators
{
    [ShellComponent]
    public class CommandCalculator : StatisticCalculator
    {
        public CommandCalculator(IStatisticListing statisticListing, IMessageBus messageBus, IErrorHandler errorHandler)
            : base(statisticListing, messageBus, errorHandler, typeof (CommandStatistic), new CommandFilter()) {}

        protected override IStatistic Process(IDEEvent @event)
        {
            var commandEvent = @event as CommandEvent;
            if (commandEvent == null)
            {
                return null;
            }

            var commandStatistic = (CommandStatistic) StatisticListing.GetStatistic(StatisticType);

            var eventCommandType = commandEvent.CommandId;

            if (commandStatistic.CommandTypeValues.ContainsKey(eventCommandType))
            {
                if (commandStatistic.CommandTypeValues[eventCommandType] != int.MaxValue)
                {
                    commandStatistic.CommandTypeValues[eventCommandType]++;
                }
            }
            else
            {
                commandStatistic.CommandTypeValues.Add(eventCommandType, 1);
            }

            return commandStatistic;
        }
    }
}
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
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Filters;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.Calculators.BaseClasses
{
    public interface IStatisticCalculator
    {
        Type StatisticType { get; }
        void InitializeStatistic();
        void Event(IDEEvent @event);
    }

    public abstract class StatisticCalculator<TStatistic> : IStatisticCalculator where TStatistic : IStatistic, new()
    {
        public Type StatisticType
        {
            get { return typeof (TStatistic); }
        }

        protected readonly IErrorHandler ErrorHandler;
        protected readonly IEventFilter EventFilter;
        protected readonly IStatisticListing StatisticListing;

        protected StatisticCalculator(IStatisticListing statisticListing,
            IMessageBus messageBus,
            IErrorHandler errorHandler,
            IEventFilter eventFilter = null)
        {
            StatisticListing = statisticListing;
            ErrorHandler = errorHandler;

            EventFilter = eventFilter ?? new NoFilter();

            messageBus.Subscribe<IDEEvent>(Event);

            if (StatisticListing.GetStatistic(StatisticType) == null)
            {
                StatisticListing.Update(new TStatistic());
            }
        }

        public void InitializeStatistic()
        {
            StatisticListing.Update(new TStatistic());
        }

        public void Event(IDEEvent @event)
        {
            var postFilterEvent = FilterEvent(@event);
            if (postFilterEvent == null)
            {
                return;
            }
            var processedStatistic = Process(postFilterEvent);

            if (processedStatistic != null)
            {
                StatisticListing.Update(processedStatistic);
            }
        }

        protected virtual IDEEvent FilterEvent(IDEEvent @event)
        {
            return EventFilter.Process(@event);
        }

        /// <summary>
        ///     Returns the statistic after processing <see cref="@event" />;
        ///     <para>Implement any calculation logic here</para>
        /// </summary>
        protected abstract IStatistic Process(IDEEvent @event);

        private class NoFilter : IEventFilter
        {
            public IDEEvent Process(IDEEvent @event)
            {
                return @event;
            }
        }
    }
}
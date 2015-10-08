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
using KaVE.Commons.Utils.Exceptions;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Filters;
using KaVE.VS.Statistics.Statistics;

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

        protected readonly ILogger ErrorHandler;
        protected readonly IEventPreprocessor EventPreprocessor;
        protected readonly IStatisticListing StatisticListing;

        protected StatisticCalculator(IStatisticListing statisticListing,
            IMessageBus messageBus,
            ILogger errorHandler,
            IEventPreprocessor preprocessor = null)
        {
            StatisticListing = statisticListing;
            ErrorHandler = errorHandler;

            EventPreprocessor = preprocessor ?? new NoPreprocessing();

            messageBus.Subscribe<IDEEvent>(Event);

            if (Equals(StatisticListing.GetStatistic<TStatistic>(), default(TStatistic)))
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
            var postPreprocessingEvent = Preprocess(@event);
            if (postPreprocessingEvent == null)
            {
                return;
            }

            var calculatedStatistic = StatisticListing.GetStatistic<TStatistic>();
            if (calculatedStatistic == null)
            {
                ErrorHandler.Error("Statistic of type {0} was not initialized in StatisticListing", typeof(TStatistic));
            }

            Calculate(calculatedStatistic, postPreprocessingEvent);
            StatisticListing.Update(calculatedStatistic);
        }

        protected virtual IDEEvent Preprocess(IDEEvent @event)
        {
            return EventPreprocessor.Preprocess(@event);
        }

        /// <summary>
        ///     Returns the statistic after processing <see cref="@event" />;
        ///     <para>Implement any calculation logic here</para>
        /// </summary>
        protected abstract void Calculate(TStatistic statistic, IDEEvent @event);

        private class NoPreprocessing : IEventPreprocessor
        {
            public IDEEvent Preprocess(IDEEvent @event)
            {
                return @event;
            }
        }
    }
}
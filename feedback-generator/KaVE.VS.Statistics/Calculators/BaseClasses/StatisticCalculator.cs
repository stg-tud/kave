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
using JetBrains.Annotations;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Filters;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.Calculators.BaseClasses
{
    public abstract class StatisticCalculator
    {
        protected readonly IErrorHandler ErrorHandler;

        protected readonly IEventFilter EventFilter;
        protected readonly IStatisticListing StatisticListing;
        protected readonly Type StatisticType;

        protected bool BlockAllEvents;

        protected StatisticCalculator(IStatisticListing statisticListing,
            IMessageBus messageBus,
            IErrorHandler errorHandler,
            Type statisticType,
            IEventFilter eventFilter = null)
        {
            StatisticListing = statisticListing;
            StatisticType = statisticType;
            ErrorHandler = errorHandler;
            EventFilter = eventFilter;

            messageBus.Subscribe<IDEEvent>(Event);

            if (StatisticListing.GetStatistic(StatisticType) == null)
            {
                Initialize();
            }
        }

        public void InitializeStatistic()
        {
            Initialize();
        }

        protected void Initialize()
        {
            var initializedStatistic = GetInitializedStatistic();
            if (initializedStatistic != null)
            {
                StatisticListing.Update(initializedStatistic);
            }
        }

        [Pure]
        protected IStatistic GetInitializedStatistic()
        {
            try
            {
                var newStatistic = (IStatistic) Activator.CreateInstance(StatisticType);
                return newStatistic;
            }
            catch (Exception e)
            {
                BlockAllEvents = true;
                ErrorHandler.SendErrorMessageToLogger(
                    e,
                    string.Format(
                        "Statistic of type {0} must implement a constructor without parameters",
                        StatisticType.Name));
                return null;
            }
        }

        /// <summary>
        ///     Sends no update if <see cref="@event" /> is null (e.g. when filtered)
        /// </summary>
        public void Event(IDEEvent @event)
        {
            if (BlockAllEvents)
            {
                return;
            }

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

        /// <summary>
        ///     Gets called before Process and returns null if the Event is filtered
        /// </summary>
        protected virtual IDEEvent FilterEvent(IDEEvent @event)
        {
            return (EventFilter == null) ? @event : EventFilter.Process(@event);
        }

        /// <summary>
        ///     Returns the statistic after processing <see cref="@event" />;
        ///     <para>Implement any calculation logic here</para>
        /// </summary>
        protected abstract IStatistic Process(IDEEvent @event);
    }
}
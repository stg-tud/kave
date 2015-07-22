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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Application;
using JetBrains.Util;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;

namespace KaVE.VS.Achievements.UI.StatisticUI
{
    [ShellComponent]
    public class StatisticViewModel : IObserver<IStatistic>
    {
        public delegate void CollectionsChangedHandler();

        private readonly IErrorHandler _errorHandler;

        private readonly IStatisticListing _statisticListing;
        private readonly IUiDelegator _uiDelegator;
        private readonly IDisposable _unsubscriber;

        public Dictionary<Type, ObservableCollection<StatisticElement>> CollectionDictionary =
            new Dictionary<Type, ObservableCollection<StatisticElement>>();

        public StatisticViewModel(IStatisticListing statisticListing,
            IObservable<IStatistic> observable,
            IUiDelegator uiDelegator,
            IErrorHandler errorHandler)
        {
            _unsubscriber = observable.Subscribe(this);
            _statisticListing = statisticListing;
            _uiDelegator = uiDelegator;
            _errorHandler = errorHandler;

            InitializeCollectionsFromStatisticListing();
        }

        public void OnNext(IStatistic value)
        {
            try
            {
                var collection = CollectionDictionary[value.GetType()];
                _uiDelegator.DelegateToStatisticUi(() => RefreshElementsForStatistic(value, collection));
            }
            catch (KeyNotFoundException e)
            {
                _errorHandler.SendErrorMessageToLogger(
                    e,
                    string.Format(
                        "Statistic with Type: {0} not initalized correctly in StatisticViewModel",
                        value.GetType()));
            }
        }

        public void OnError(Exception error)
        {
            // Do nothing
        }

        public void OnCompleted()
        {
            // Do nothing
        }

        public static event CollectionsChangedHandler CollectionsChanged;

        private static void OnCollectionsChanged()
        {
            var handler = CollectionsChanged;
            if (handler != null)
            {
                handler();
            }
        }

        private void InitializeCollectionsFromStatisticListing()
        {
            var statisticDictionary = _statisticListing.StatisticDictionary;
            foreach (var o in statisticDictionary)
            {
                var collection = new ObservableCollection<StatisticElement>();
                AddElementsToCollection(o.Value, collection);
                CollectionDictionary.Add(o.Key, collection);
            }
        }

        private static void AddElementsToCollection(IStatistic statistic, ICollection<StatisticElement> collection)
        {
            var statisticPropertyCollection = statistic.GetCollection();
            collection.AddRange(statisticPropertyCollection);
            OnCollectionsChanged();
        }

        public void RefreshElementsForStatistic(IStatistic statistic, ObservableCollection<StatisticElement> collection)
        {
            collection.Clear();
            AddElementsToCollection(statistic, collection);
        }

        public void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        public void ResetCollections()
        {
            CollectionDictionary.Clear();
            InitializeCollectionsFromStatisticListing();
        }
    }
}
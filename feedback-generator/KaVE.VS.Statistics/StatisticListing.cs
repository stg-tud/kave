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
using System.IO;
using JetBrains.Application;
using KaVE.JetBrains.Annotations;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics
{
    public interface IStatisticListing : IListing<Type, IStatistic>
    {
        Dictionary<Type, IStatistic> StatisticDictionary { get; }
        bool BlockUpdate { get; set; }

        TStatistic GetStatistic<TStatistic>() where TStatistic : IStatistic;

        void Update(IStatistic statistic);

        void SendUpdateWithAllStatistics();
    }

    [ShellComponent]
    public class StatisticListing : Listing<Type, IStatistic>, IObservable<IStatistic>, IStatisticListing
    {
        private const string FileName = "statistics";

        private static readonly string AppDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
        private static readonly string DirectoryPath = Path.Combine(AppDataPath, "KaVE");

        private readonly List<IObserver<IStatistic>> _observers;

        public StatisticListing() : base(FileName, DirectoryPath)
        {
            _observers = new List<IObserver<IStatistic>>();
        }

        public IDisposable Subscribe(IObserver<IStatistic> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer);
        }

        public Dictionary<Type, IStatistic> StatisticDictionary
        {
            get { return Dictionary; }
        }
        public bool BlockUpdate { get; set; }

        [CanBeNull]
        public TStatistic GetStatistic<TStatistic>() where TStatistic : IStatistic
        {
            try
            {
                return (TStatistic) GetValue(typeof(TStatistic));
            }
            catch (KeyNotFoundException)
            {
                return default(TStatistic);
            }
        }

        /// <summary>
        ///     Implicitly persists the listing
        /// </summary>
        public void Update(IStatistic statistic)
        {
            Update(statistic.GetType(), statistic);

            SendUpdate(statistic);
        }

        public void SendUpdateWithAllStatistics()
        {
            foreach (var keyValuePair in Dictionary)
            {
                SendUpdate(keyValuePair.Value);
            }
        }

        private void SendUpdate(IStatistic statistic)
        {
            if (BlockUpdate)
            {
                return;
            }

            var observers = new List<IObserver<IStatistic>>();
            observers.AddRange(_observers);

            foreach (var observer in observers)
            {
                observer.OnNext(statistic);
            }
        }


        // ---------------------
        // Observerable Methods:
        // ---------------------

        private class Unsubscriber : IDisposable
        {
            private readonly IObserver<IStatistic> _observer;
            private readonly List<IObserver<IStatistic>> _observers;

            public Unsubscriber(List<IObserver<IStatistic>> observers,
                IObserver<IStatistic> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
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
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;

namespace KaVE.VS.Achievements.Statistics.Listing
{
    [ShellComponent]
    public class StatisticListing : Listing<Type, IStatistic>, IObservable<IStatistic>, IStatisticListing
    {
        private const string ProjectName = "KaVEAchievements";

        private const string FileName = "statistics";

        private static readonly string AppDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        private static readonly string DirectoryPath = Path.Combine(AppDataPath, ProjectName);
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

        public bool BlockUpdateToObservers { get; set; }

        public Dictionary<Type, IStatistic> StatisticDictionary
        {
            get { return Dictionary; }
        }

        [CanBeNull]
        public IStatistic GetStatistic(Type type)
        {
            try
            {
                return GetValue(type);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        ///     Implicitly persists the listing
        /// </summary>
        public void Update(IStatistic o)
        {
            Update(o.GetType(), o);

            SendUpdateToObservers(o);
        }

        public void SendUpdateToObserversWithAllStatistics()
        {
            foreach (var keyValuePair in StatisticDictionary)
            {
                SendUpdateToObservers(keyValuePair.Value);
            }
        }

        private void SendUpdateToObservers(IStatistic statistic)
        {
            if (BlockUpdateToObservers)
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
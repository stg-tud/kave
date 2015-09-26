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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using KaVE.VS.Achievements.Utils;
using KaVE.VS.Statistics.Utils;
using Newtonsoft.Json;

namespace KaVE.VS.Achievements.BaseClasses.AchievementTypes
{
    /// <summary>
    ///     Base class for modelling any Achievement with progress
    ///     (e.g. open a file ten times)
    /// </summary>
    public class ProgressAchievement : BaseAchievement, INotifyPropertyChanged
    {
        [JsonIgnore]
        private readonly ITargetValueProvider _targetValueProvider;

        private double _currentProgress;
        private object _currentValue;

        /// <summary>
        ///     Gets the <see cref="_targetValueProvider" /> component from the <see cref="Registry" />
        /// </summary>
        /// <param name="id">The ID of the Achievement</param>
        public ProgressAchievement(int id) : base(id)
        {
            _targetValueProvider = Registry.GetComponent<ITargetValueProvider>();
            Initialize();
        }

        [JsonIgnore]
        public object TargetValue
        {
            get { return _targetValueProvider.GetTargetValue(Id); }
        }

        /// <summary>
        ///     The Progress of the Achievement as
        ///     <see cref="CurrentValue" />/<see cref="TargetValue" /> or
        ///     <see cref="TargetValue" />/<see cref="TargetValue" />
        ///     if the <see cref="CurrentValue" /> is greater than <see cref="TargetValue" />
        /// </summary>
        [JsonIgnore]
        public string ProgressString
        {
            get
            {
                return CurrentProgress < 100
                    ? string.Format(
                        "{0}/{1}",
                        CurrentValue.Format(),
                        TargetValue.Format())
                    : string.Format("{0}/{0}", TargetValue.Format());
            }
        }

        public object CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                OnPropertyChanged("ProgressString");
            }
        }

        /// <summary>
        ///     The Current Progress;
        ///     Can't be greater than 100;
        /// </summary>
        public double CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value < 100 ? value : 100;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Raises a PropertyChanged Event when the ProgressString or the CurrentProgress is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Sets the initial CurrentValue to an empty string
        ///     if creating a new instance of type of <see cref="TargetValue" /> fails
        /// </summary>
        public new void Initialize()
        {
            try
            {
                CurrentValue = Activator.CreateInstance(TargetValue.GetType());
            }
            catch
            {
                CurrentValue = "-";
            }

            base.Initialize();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
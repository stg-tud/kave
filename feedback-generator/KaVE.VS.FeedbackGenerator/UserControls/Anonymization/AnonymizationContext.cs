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

using System.ComponentModel;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.Settings;

namespace KaVE.VS.FeedbackGenerator.UserControls.Anonymization
{
    public class AnonymizationContext : INotifyPropertyChanged
    {
        private readonly AnonymizationSettings _anonymizationSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool RemoveCodeNames
        {
            get { return _anonymizationSettings.RemoveCodeNames; }
            set
            {
                _anonymizationSettings.RemoveCodeNames = value;
                OnPropertyChanged("RemoveCodeNames");
            }
        }

        public bool RemoveDurations
        {
            get { return _anonymizationSettings.RemoveDurations; }
            set
            {
                _anonymizationSettings.RemoveDurations = value;
                OnPropertyChanged("RemoveDurations");
            }
        }

        public bool RemoveStartTimes
        {
            get { return _anonymizationSettings.RemoveStartTimes; }
            set
            {
                _anonymizationSettings.RemoveStartTimes = value;
                OnPropertyChanged("RemoveStartTimes");
            }
        }

        public bool RemoveSessionIDs
        {
            get { return _anonymizationSettings.RemoveSessionIDs; }
            set
            {
                _anonymizationSettings.RemoveSessionIDs = value;
                OnPropertyChanged("RemoveSessionIDs");
            }
        }

        public AnonymizationContext(AnonymizationSettings anonymizationSettings)
        {
            _anonymizationSettings = anonymizationSettings;
        }
    }
}
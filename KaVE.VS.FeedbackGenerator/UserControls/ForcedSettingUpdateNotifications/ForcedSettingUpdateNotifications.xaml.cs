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
using System.Windows;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.UserControls.ForcedSettingUpdateNotifications
{
    public partial class ForcedSettingUpdateNotifications : INotifyPropertyChanged
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            private set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public ForcedSettingUpdateNotifications(string notificationText)
        {
            Text = notificationText;
            DataContext = this;
            InitializeComponent();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
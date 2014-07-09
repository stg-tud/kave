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
using System.ComponentModel;
using System.Linq.Expressions;
using KaVE.Utils.Reflection;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : ViewModelBase<T>
    {
        private readonly IDictionary<object, string> _propertyNameDictionary =
            new Dictionary<object, string>();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private bool _isBusy;
        private string _busyMessage;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (!_propertyNameDictionary.ContainsKey(expression))
            {
                var propertyName = TypeExtensions<T>.GetPropertyName(expression);
                _propertyNameDictionary.Add(expression, propertyName);
            }

            OnPropertyChanged(_propertyNameDictionary[expression]);
        }

        protected void SetBusy(string reason)
        {
            BusyMessage = reason;
            IsBusy = true;
        }

        protected void SetIdle()
        {
            IsBusy = false;
        }

        /// <summary>
        ///     Indicates that the view is busy performing some background task. No calls should be issued on the model if this is
        ///     true.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                OnPropertyChanged(vm => vm.IsBusy);
            }
        }

        public string BusyMessage
        {
            get { return _busyMessage; }
            private set
            {
                _busyMessage = value;
                OnPropertyChanged(vm => vm.BusyMessage);
            }
        }
    }
}
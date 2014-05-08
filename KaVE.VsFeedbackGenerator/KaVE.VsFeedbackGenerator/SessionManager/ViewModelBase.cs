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
    public abstract class ViewModelBase<T> : INotifyPropertyChanged
    {
        private readonly IDictionary<object, string> _propertyNameDictionary =
            new Dictionary<object, string>();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
    }
}
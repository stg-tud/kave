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
using System.Linq.Expressions;

namespace KaVE.Commons.Utils.Reflection
{
    public static class PropertyChangedEx
    {
        public static void OnPropertyChanged<T, TProperty>(this T self, Expression<Func<T, TProperty>> expression, Action<TProperty> handler) where T : INotifyPropertyChanged
        {
            self.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(TypeExtensions<T>.GetPropertyName(expression)))
                {
                    handler(expression.Compile()(self));
                }
            };
        }
    }
}
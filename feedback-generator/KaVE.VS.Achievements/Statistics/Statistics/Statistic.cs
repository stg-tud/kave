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
using System.Linq.Expressions;
using System.Resources;
using KaVE.Commons.Utils.Reflection;
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Achievements.Statistics.Calculators.BaseClasses;
using KaVE.VS.Achievements.UI.StatisticUI;

namespace KaVE.VS.Achievements.Statistics.Statistics
{
    /// <summary>
    ///     Interface for data structures of Statistics
    ///     Must be computed by <see cref="StatisticCalculator" />
    /// </summary>
    public abstract class Statistic<T> : IStatistic
    {
        /// <summary>
        ///     Used for acquiring the property names of statistics when creating StatisticElements
        /// </summary>
        protected ResourceManager PropertyNameManager = StatisticPropertyNames.ResourceManager;

        /// <summary>
        ///     Returns a List of StatisticElements containing each statistic of this data structure
        /// </summary>
        public abstract List<StatisticElement> GetCollection();

        /// <summary>
        ///     Creates a new StatisticElement containg the property name and its value
        /// </summary>
        protected StatisticElement NewElement<TProperty>(Expression<Func<T, TProperty>> expression, string value)
        {
            var propertyName = TypeExtensions<T>.GetPropertyName(expression);
            return new StatisticElement
            {
                Name = PropertyNameManager.GetString(propertyName),
                Value = value
            };
        }
    }
}
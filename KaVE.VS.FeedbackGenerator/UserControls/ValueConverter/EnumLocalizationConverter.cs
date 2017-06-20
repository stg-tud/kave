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
using System.Globalization;
using System.Windows.Data;
using KaVE.VS.FeedbackGenerator.Properties;

namespace KaVE.VS.FeedbackGenerator.UserControls.ValueConverter
{
    internal class EnumLocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var simpleType = value.GetType().Name;
            var valueName = value.ToString();
            var key = string.Format("{0}_{1}", simpleType, valueName);

            var res = EnumLocalization.ResourceManager.GetString(key);
            return res ?? string.Format("%{0}%", key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
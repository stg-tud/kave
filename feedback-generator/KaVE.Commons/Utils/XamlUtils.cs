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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;

namespace KaVE.Commons.Utils
{
    public static class XamlUtils
    {
        private const string DataTemplateBegin =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock xml:space=\"preserve\">";

        private const string DataTemplateEnd = "</TextBlock></DataTemplate>";

        /// <exception cref="System.Windows.Markup.XamlParseException">If markup is invalid.</exception>
        public static DataTemplate CreateDataTemplateFromXaml(string xaml)
        {
            return (DataTemplate) XamlReader.Parse(DataTemplateBegin + xaml + DataTemplateEnd);
        }

        private static readonly Regex NodeRegex = new Regex("<.*?>", RegexOptions.Compiled);

        public static string StripTags(this string xaml)
        {
            return NodeRegex.Replace(xaml, String.Empty);
        }

        public static string EncodeSpecialChars(this string xaml)
        {
            // the order of the replacements are importent, so be careful while reordering the existing or adding new replacements
            // We don't replace \" and ' because both characters work well with Xaml
            return xaml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
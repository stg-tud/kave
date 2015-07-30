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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using JetBrains.Reflection;
using KaVE.VS.Statistics.Properties;

namespace KaVE.VS.Statistics.UI.Utils
{
    public class StatisticElementComparer : IComparer
    {
        private static readonly string NameOfStatisticElement = UIText.Name;
        private static readonly string DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private static readonly string NumericPattern = string.Format("[^-{0}0-9]", DecimalSeparator);

        private static readonly string[] DescendingTimeSpanUnits =
        {
            TimeUnits.Days,
            TimeUnits.Hours,
            TimeUnits.Minutes,
            TimeUnits.Seconds,
            TimeUnits.Milliseconds
        };

        private readonly ListSortDirection _direction;
        private readonly string _primaryCriteria;

        public StatisticElementComparer(string primaryCriteria, ListSortDirection direction)
        {
            _primaryCriteria = primaryCriteria;
            _direction = direction;
        }

        private int DirectionMultiplier
        {
            get { return _direction == ListSortDirection.Ascending ? 1 : -1; }
        }

        public int Compare(object x, object y)
        {
            string s1, s2;
            if (!TrySetComparableElements(x, y, out s1, out s2))
            {
                return 0;
            }

            var result = _primaryCriteria == NameOfStatisticElement || (!IsNumber(s1) && !IsNumber(s2))
                ? CompareNonNumerals(s1, s2)
                : CompareNumerals(s1, s2);

            return result*DirectionMultiplier;
        }

        private static int CompareNonNumerals(string s1, string s2)
        {
            return string.Compare(s1, s2, StringComparison.Ordinal);
        }

        private static int CompareNumerals(string s1, string s2)
        {
            return GetTimeUnit(s1) == null && GetTimeUnit(s2) == null
                ? CompareNumbers(s1, s2)
                : CompareTimeSpans(s1, s2);
        }

        private static int CompareNumbers(string s1, string s2)
        {
            s1 = RemoveNonNumericals(s1);
            s2 = RemoveNonNumericals(s2);

            double number1, number2;
            return double.TryParse(s1, out number1) && double.TryParse(s2, out number2)
                ? number1.CompareTo(number2)
                : CompareBigInteger(s1, s2);
        }

        private static int CompareBigInteger(string s1, string s2)
        {
            BigInteger number1, number2;
            return BigInteger.TryParse(s1, out number1) && BigInteger.TryParse(s2, out number2)
                ? number1.CompareTo(number2)
                : 0;
        }

        private static int CompareTimeSpans(string s1, string s2)
        {
            var unit1 = GetTimeUnit(s1);
            var unit2 = GetTimeUnit(s2);

            foreach (var timeSpanUnit in DescendingTimeSpanUnits)
            {
                if (timeSpanUnit.Equals(unit1) && !timeSpanUnit.Equals(unit2))
                {
                    return 1;
                }
                if (timeSpanUnit.Equals(unit2) && !timeSpanUnit.Equals(unit1))
                {
                    return -1;
                }
            }

            return CompareNumbers(s1, s2);
        }

        private bool TrySetComparableElements(object x, object y, out string s1, out string s2)
        {
            var statElement1 = x as StatisticElement;
            var statElement2 = y as StatisticElement;

            s1 = statElement1 == null ? null : statElement1.GetFieldOrPropertyValue(_primaryCriteria) as string;
            s2 = statElement2 == null ? null : statElement2.GetFieldOrPropertyValue(_primaryCriteria) as string;

            return s1 != null && s2 != null;
        }

        private static bool IsNumber(string s1)
        {
            return s1.Any(char.IsDigit);
        }

        private static string RemoveNonNumericals(string s)
        {
            return Regex.Replace(s, NumericPattern, "");
        }

        private static string GetTimeUnit(string s)
        {
            return DescendingTimeSpanUnits.Where(s.Contains).LastOrDefault();
        }
    }
}
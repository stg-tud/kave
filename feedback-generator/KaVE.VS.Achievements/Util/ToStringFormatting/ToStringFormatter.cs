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
using System.Numerics;
using KaVE.VS.Achievements.Properties;

namespace KaVE.VS.Achievements.Util.ToStringFormatting
{
    public static class ToStringFormatter
    {
        public static string Format(this int number)
        {
            return number.ToString(FormatStrings.Integer);
        }

        public static string Format(this double number)
        {
            return number.ToString(FormatStrings.Double);
        }

        public static string Format(this TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
            {
                return Format(timeSpan.TotalDays) + " " + TimeUnits.Days;
            }
            if (timeSpan.Hours > 0)
            {
                return Format(timeSpan.TotalHours) + " " + TimeUnits.Hours;
            }
            if (timeSpan.Minutes > 0)
            {
                return Format(timeSpan.TotalMinutes) + " " + TimeUnits.Minutes;
            }
            if (timeSpan.Seconds > 0)
            {
                return Format(timeSpan.TotalSeconds) + " " + TimeUnits.Seconds;
            }
            return Format(timeSpan.TotalMilliseconds) + " " + TimeUnits.Milliseconds;
        }

        public static string Format(this BigInteger number)
        {
            return number.ToString(FormatStrings.Integer);
        }
    }
}
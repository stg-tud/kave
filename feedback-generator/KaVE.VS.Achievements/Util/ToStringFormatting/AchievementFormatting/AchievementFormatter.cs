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

namespace KaVE.VS.Achievements.Util.ToStringFormatting.AchievementFormatting
{
    public static class AchievementFormatter
    {
        public static string Format(this object o)
        {
            var result = o.ToString();

            if (o is int)
            {
                result = IntegerToFormattedString((int) o);
            }

            if (o is TimeSpan)
            {
                result = TimeSpanToFormattedString((TimeSpan) o);
            }

            return result;
        }

        private static string IntegerToFormattedString(this int i)
        {
            return ToStringFormatter.Format(i);
        }

        private static string TimeSpanToFormattedString(this TimeSpan t)
        {
            return ToStringFormatter.Format(t);
        }
    }
}
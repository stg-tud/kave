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

namespace KaVE.Commons.Utils.IO
{
    public static class FileSystemEx
    {
        public static readonly string[] Units = {"B", "KB", "MB"};

        public static string FormatSizeInBytes(this long sizeInBytes)
        {
            return sizeInBytes.FormatSizeInBytes(CultureInfo.CurrentCulture);
        }

        public static string FormatSizeInBytes(this long sizeInBytes, CultureInfo culture)
        {
            if (sizeInBytes == 0)
            {
                return string.Format("0 {0}", Units[0]);
            }
            var magnitude = Math.Min(Convert.ToInt32(Math.Floor(Math.Log(sizeInBytes, 1024))), Units.Length - 1);
            var unit = Units[magnitude];
            var sizeInUnit = Math.Round(sizeInBytes/Math.Pow(1024, magnitude), 1);
            return string.Format(culture, "{0:#,##0.#} {1}", sizeInUnit, unit);
        }
    }
}
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
 * 
 * Contributors:
 *    - Sebastian Proksch
 *    - Dennis Albrecht
 *    - Sven Amann
 */

using System;
using System.Globalization;
using System.Linq;

namespace KaVE.Commons.Utils
{
    public static class StringUtils
    {
        public static byte[] AsBytes(this string str)
        {
            var bytes = new byte[str.Length*sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string AsString(this byte[] bytes)
        {
            var chars = new char[bytes.Length/sizeof (char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static Boolean Contains(this string value,
            string needle,
            CompareOptions compareOptions = CompareOptions.None)
        {
            var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            return compareInfo.IndexOf(value, needle, compareOptions) >= 0;
        }

        public static bool ContainsAny(this string value, params string[] needles)
        {
            return needles.Any(value.Contains);
        }
    }
}
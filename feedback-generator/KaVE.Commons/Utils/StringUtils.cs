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
using System.Globalization;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils
{
    public static class StringUtils
    {
        public static byte[] AsBytes(this string str)
        {
            var bytes = new byte[str.Length*sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string AsString(this byte[] bytes)
        {
            var chars = new char[bytes.Length/sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static bool Contains(this string value,
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

        public static bool StartsWithAny(this string value, IList<string> needles, StringComparison options)
        {
            return needles.Any(n => n.StartsWith(value, options));
        }

        [StringFormatMethod("value")]
        public static string FormatEx(this string value, params object[] args)
        {
            return string.Format(value, args);
        }

        public static int FindNext(this string str, int currentIndex, params char[] characters)
        {
            AssertIndexBoundaries(str, currentIndex);
            for (var i = currentIndex; i < str.Length; i++)
            {
                var c = str[i];
                if (characters.Contains(c))
                {
                    return i;
                }
            }
            return -1;
        }

        private static void AssertIndexBoundaries(string str, int currentIndex)
        {
            Asserts.NotNull(str);
            if (currentIndex < 0 || currentIndex >= str.Length)
            {
                Asserts.Fail("index '{0}' is out of bounds for string '{1}'", currentIndex, str);
            }
        }

        public static int FindPrevious(this string str, int currentIndex, params char[] characters)
        {
            AssertIndexBoundaries(str, currentIndex);
            for (var i = currentIndex; i >= 0; i--)
            {
                var c = str[i];
                if (characters.Contains(c))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindCorrespondingOpenBracket(this string str, int currentIndex)
        {
            const string closeBrackets = "]}>)";
            AssertIndexBoundaries(str, currentIndex);
            if (!closeBrackets.Contains(str[currentIndex]))
            {
                Asserts.Fail("invalid closing bracket at index '{0}' for string '{1}'".FormatEx(currentIndex, str));
            }

            var open = str[currentIndex];
            var close = open.GetCorresponding();

            var depth = 0;
            for (var i = currentIndex; i > 0; i--)
            {
                depth = UpdateDepth(depth, open, close, str[i]);
                if (depth == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int UpdateDepth(int depth, char open, char close, char current)
        {
            if (current == open)
            {
                return depth + 1;
            }
            if (current == close)
            {
                return depth - 1;
            }
            return depth;
        }

        public static int FindCorrespondingCloseBracket(this string str, int currentIndex)
        {
            const string closeBrackets = "[{<(";
            AssertIndexBoundaries(str, currentIndex);
            if (!closeBrackets.Contains(str[currentIndex]))
            {
                Asserts.Fail("invalid closing bracket at index '{0}' for string '{1}'".FormatEx(currentIndex, str));
            }

            var open = str[currentIndex];
            var close = open.GetCorresponding();

            var depth = 0;
            for (var i = currentIndex; i < str.Length; i++)
            {
                depth = UpdateDepth(depth, open, close, str[i]);
                if (depth == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static char GetCorresponding(this char c)
        {
            switch (c)
            {
                case '(':
                    return ')';
                case ')':
                    return '(';
                case '{':
                    return '}';
                case '}':
                    return '{';
                case '[':
                    return ']';
                case ']':
                    return '[';
                case '<':
                    return '>';
                case '>':
                    return '<';
                default:
                    throw new ArgumentException(string.Format("no supported bracket type: {0}", c));
            }
        }

        public static string TakeUntil(this string s, params char[] stopChars)
        {
            var count = s.TakeWhile(c => !stopChars.Contains(c)).Count();
            return s.Substring(0, count);
        }
    }
}
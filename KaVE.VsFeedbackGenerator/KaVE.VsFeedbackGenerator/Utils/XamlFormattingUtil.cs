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
using System.Linq;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class XamlFormattingUtil
    {
        private const string BoldTag = "Bold";
        private const string ItalicTag = "Italic";
        private const string TextTag = "Span";
        private const string ForegroundParameter = "Foreground";

        private static string FormatStringParameter(string key, string value)
        {
            return string.Format("{0}=\"{1}\"", key, value);
        }

        private static string Create(string type, string content, params string[] parameter)
        {
            return string.Format("<{0}{2}>{1}</{0}>", type, content, string.Concat(parameter.Select(p => " " + p)));
        }

        public static string Bold(string content)
        {
            return Create(BoldTag, content);
        }

        public static string Bold(string content, string color)
        {
            return Create(BoldTag, content, FormatStringParameter(ForegroundParameter, color));
        }

        public static string Colored(string content, string color)
        {
            return Create(TextTag, content, FormatStringParameter(ForegroundParameter, color));
        }

        public static string Italic(string content)
        {
            return Create(ItalicTag, content);
        }

        public static string Italic(string content, string color)
        {
            return Create(ItalicTag, content, FormatStringParameter(ForegroundParameter, color));
        }
    }
}
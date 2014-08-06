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
using System.Linq;
using System.Reflection;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    internal static class IDEEventDetailsToJsonConverter
    {
        private static readonly PropertyInfo[] IDEEventProperties = typeof (IDEEvent).GetProperties();
        private static readonly string NewLine = Environment.NewLine;

        public static string GetDetailsAsJson(this IDEEvent ideEvent)
        {
            return ideEvent.ToPrettyPrintJson()
                           .GetContentLines()
                           .Where(IsSpecializedEventInformation)
                           .Select(WithoutComma)
                           .Join(NewLine);
        }

        private static IEnumerable<string> GetContentLines(this string details)
        {
            var lines = details.Split(NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            // remove first opening and last closing brace
            return lines.Skip(1).Take(lines.Length - 2);
        }

        /// <summary>
        ///     A line is considered to contain specialized information, if it does not contain the name of the property of
        ///     the <see cref="IDEEvent" /> type, like, for example, "  'IDESessionUUID': '0xDEADBEEF'" does.
        /// </summary>
        private static bool IsSpecializedEventInformation(string detailLine)
        {
            return IDEEventProperties.All(ideEventProperty => !detailLine.Contains(ideEventProperty.Name));
        }

        private static string WithoutComma(string arg)
        {
            return arg.EndsWith(",") ? arg.Substring(0, arg.Length - 1) : arg;
        }
    }
}
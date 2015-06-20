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
 *    - Andreas Bauer
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Json;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation
{
    public static class IDEEventDetailsToJsonConverter
    {
        private static readonly IList<string> IDEEventPropertyNames =
            typeof (IDEEvent).GetProperties().Select(p => p.Name).ToList();

        private static readonly string NewLine = Environment.NewLine;

        public static string GetDetailsAsJson(this IDEEvent ideEvent)
        {
            var hiddenProperties = new List<string>(IDEEventPropertyNames);

            if (ideEvent is CompletionEvent)
            {
                hiddenProperties.Add("Context2");
                hiddenProperties.Add("Selections");
                hiddenProperties.Add("ProposalCollection");
            }

            return ideEvent.ToPrettyPrintJson(hiddenProperties)
                           .GetContentLines()
                           .Select(WithoutComma)
                           .Join(NewLine);
        }

        private static IEnumerable<string> GetContentLines(this string details)
        {
            var lines = details.Split(NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            // remove first opening and last closing brace
            return lines.Skip(1).Take(lines.Length - 2);
        }

        private static string WithoutComma(string arg)
        {
            return arg.EndsWith(",") ? arg.Substring(0, arg.Length - 1) : arg;
        }
    }
}
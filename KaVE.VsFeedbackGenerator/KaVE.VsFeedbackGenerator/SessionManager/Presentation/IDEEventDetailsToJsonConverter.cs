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
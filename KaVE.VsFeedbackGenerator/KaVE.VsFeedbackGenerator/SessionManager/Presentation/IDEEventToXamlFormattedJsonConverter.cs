using System.Text.RegularExpressions;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Utils.Serialization;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Util = KaVE.VsFeedbackGenerator.Utils.XamlFormattingUtil;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public static class IDEEventToXamlFormattedJsonConverter
    {
        public static string ToXamlFormattedJson(this IDEEvent ideEvent)
        {
            if (ideEvent == null)
            {
                return null;
            }
            var rawVersion = ToJson(ideEvent);
            var highlighted = Highlight(rawVersion);
            return highlighted;
        }

        private static string ToJson([NotNull] IDEEvent ideEvent)
        {
            var regex = new Regex("\n *");
            var json = ideEvent.ToJson(new NameToIdentifierConverter(), new EnumToStringConverter());
            return regex.Replace(json, match => match.Value + match.Value.Substring(1));
        }

        internal static string Highlight([NotNull] string xaml)
        {
            return Regex.Replace(
                xaml,
                @"(""(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\""])*""(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)",
                match => CreateReplacement(match.Value));
        }

        private static string CreateReplacement(string match)
        {
            if (IsStringConstant(match))
            {
                return IsPropertyKey(match) ? FormatPropertyKey(match) : FormatStringConstant(match);
            }
            if (IsBooleanConstant(match))
            {
                return FormatBooleanConstant(match);
            }
            if (IsNullConstant(match))
            {
                return FormatNullConstant(match);
            }
            return FormatNumberConstant(match);
        }

        private static string FormatStringConstant(string match)
        {
            return Util.Colored(match, "Blue");
        }

        private static string FormatPropertyKey(string match)
        {
            return Util.Bold(match, "Blue");
        }

        private static string FormatBooleanConstant(string match)
        {
            return Util.Bold(match, "Darkred");
        }

        private static string FormatNullConstant(string match)
        {
            return Util.Bold(match);
        }

        private static string FormatNumberConstant(string match)
        {
            return Util.Colored(match, "Darkgreen");
        }

        private static bool IsStringConstant(string match)
        {
            return Regex.IsMatch(match, "^\"");
        }

        private static bool IsPropertyKey(string match)
        {
            return Regex.IsMatch(match, ":$");
        }

        private static bool IsBooleanConstant(string match)
        {
            return Regex.IsMatch(match, "true|false");
        }

        private static bool IsNullConstant(string match)
        {
            return Regex.IsMatch(match, "null");
        }
    }
}
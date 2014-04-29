using System.Text.RegularExpressions;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Utils.Serialization;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public static class IDEEventToXamlFormattedJsonConverter
    {
        private const string BoldFontTag = "Bold";
        private const string NormalFontTag = "Span";
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
            string tag, color;
            if (IsStringConstant(match))
            {
                tag = IsPropertyKey(match) ? BoldFontTag : NormalFontTag;
                color = "Blue";
            }
            else if (IsBooleanConstant(match))
            {
                tag = BoldFontTag;
                color = "Darkred";
            }
            else if (IsNullConstant(match))
            {
                tag = BoldFontTag;
                color = "Black";
            }
            else
            {
                tag = NormalFontTag;
                color = "Darkgreen";
            }
            return "<" + tag + " Foreground=\"" + color + "\">" + match + "</" + tag + ">";
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
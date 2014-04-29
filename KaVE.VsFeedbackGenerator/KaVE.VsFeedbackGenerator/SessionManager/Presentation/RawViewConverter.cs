using System.Text.RegularExpressions;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    // TODO @Dennis: IDEEventToXamlFormattedJsonConverter?
    public static class RawViewConverter
    {
        private const string BoldFontTag = "Bold";
        private const string NormalFontTag = "Span";
        // TODO @Dennis: ToXamlFormattedJson?
        public static string ToXaml(this IDEEvent ideEvent)
        {
            if (ideEvent == null)
            {
                return null;
            }
            var rawversion = ToJson(ideEvent);
            var correctlyIndendet = AdjustIndent(rawversion);
            var highlighted = Highlight(correctlyIndendet);
            return highlighted;
        }

        private static string ToJson([NotNull] IDEEvent ideEvent)
        {
            // TODO @Dennis: Guck dir mal an was der DetailsView beim Umwandeln in Json so macht, da sind Converter die hier fehlen und das ist nicht getestet
            return JsonConvert.SerializeObject(ideEvent, Formatting.Indented).Replace("  ", "    ");
        }

        internal static string AdjustIndent([NotNull] string xaml)
        {
            // TODO @Dennis: Denk mal darüber nach was das Replace in der Method obendrüber macht und schmeiss dann die Methode hier raus...
            var regex = new Regex("\n *");
            return regex.Replace(xaml, match => match.Value + match.Value.Substring(1));
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
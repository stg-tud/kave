using System.Text.RegularExpressions;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public static class RawViewConverter
    {
        public static string ToXaml(this IDEEvent ideEvent)
        {
            if (ideEvent == null)
            {
                return null;
            }
            var rawversion = RawVersion(ideEvent);
            var correctlyIndendet = AdjustIndent(rawversion);
            var highlighted = Highlight(correctlyIndendet);
            return highlighted;
        }

        private static string RawVersion([NotNull] IDEEvent ideEvent)
        {
            return JsonConvert.SerializeObject(ideEvent, Formatting.Indented).Replace("  ", "    ");
        }

        internal static string AdjustIndent([NotNull] string xaml)
        {
            var regex = new Regex("\n *");
            return regex.Replace(xaml, match => match.Value + match.Value.Substring(1));
        }

        internal static string Highlight([NotNull] string xaml)
        {
            return Regex.Replace(
                xaml,
                @"(¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)"
                    .Replace('¤', '"'),
                match =>
                {
                    var cls = "<Span Foreground=\"Darkgreen\">" + match.Value + "</Span>";
                    if (Regex.IsMatch(match.Value, "^\""))
                    {
                        var type = Regex.IsMatch(match.Value, ":$") ? "Bold" : "Span";
                        cls = "<" + type + " Foreground=\"Blue\">" + match.Value + "</" + type + ">";
                    }
                    else if (Regex.IsMatch(match.Value, "true|false"))
                    {
                        cls = "<Bold Foreground=\"Darkred\">" + match.Value + "</Bold>";
                    }
                    else if (Regex.IsMatch(match.Value, "null"))
                    {
                        cls = "<Bold>null</Bold>";
                    }
                    return cls;
                });
        }
    }
}
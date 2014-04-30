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
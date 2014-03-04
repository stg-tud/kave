using System.Linq;
using System.Text;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    static class ContextVisualizationConverterFixtures
    {
        internal const string LineBreak = ContextVisualizationConverter.LineBreak;
        internal const string Indent = ContextVisualizationConverter.Indent;
        internal const string CompletionMarker = ContextVisualizationConverter.CompletionMarker;
        internal const string Class = "<Bold>class</Bold>";
        internal const string Inherits = "<Bold> : </Bold>";

        internal const string GetMethod =
            "[N.Return, Assembly, Version=1.0.0.0] [N.Class, Assembly, Version=1.0.0.0].Method([N.Argument, Assembly, Version=1.0.0.0] argument)";

        internal static string CreateType(string type = "Class")
        {
            return string.Format("N.{0}, Assembly, Version=1.0.0.0", type);
        }

        internal static string Line(params string[] elems)
        {
            return elems.Aggregate(new StringBuilder(), (builder, s) => builder.Append(s)).Append(LineBreak).ToString();
        }

        internal static string Bold(string el)
        {
            return "<Bold>" + el + "</Bold>";
        }
    }
}

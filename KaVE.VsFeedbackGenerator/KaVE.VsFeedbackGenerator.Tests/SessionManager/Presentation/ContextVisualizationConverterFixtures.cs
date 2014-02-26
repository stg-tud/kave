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
            "[Namespace.Return, Assembly, Version=1.0.0.0] [Namespace.Class, Assembly, Version=1.0.0.0].Method([Namespace.Argument, Assembly, Version=1.0.0.0] argument)";

        internal static string CreateType(string type = "Class")
        {
            return string.Format("Namespace.{0}, Assembly, Version=1.0.0.0", type);
        }

        internal static string Line(params string[] elems)
        {
            return elems.Aggregate(new StringBuilder(), (builder, s) => builder.Append(s)).Append(LineBreak).ToString();
        }
    }
}

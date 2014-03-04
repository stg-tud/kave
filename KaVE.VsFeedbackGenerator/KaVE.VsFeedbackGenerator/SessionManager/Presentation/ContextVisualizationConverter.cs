using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    internal static class ContextVisualizationConverter
    {
        internal const string LineBreak = "\r\n";
        internal const string Indent = "  ";
        internal const string Space = " ";
        internal const string CurlyBracketOpen = "{";
        internal const string CurlyBracketClose = "}";
        internal const string BoldStart = "<Bold>";
        internal const string BoldEnd = "</Bold>";
        internal const string CompletionMarker = "<Italic Foreground=\"Blue\">@Completion</Italic>";

        internal static string ToXaml(this Context context)
        {
            if (context == null || context.EnclosingClassHierarchy == null)
            {
                return null;
            }

            var builder = new StringBuilder();

            builder.AppendLine(0, b => b.AppendHierarchy(context.EnclosingClassHierarchy));
            builder.AppendLine(0, CurlyBracketOpen);
            if (context.EnclosingMethodDeclaration == null)
            {
                builder.AppendLine(1, CompletionMarker);
            }
            else
            {
                builder.AppendLine(1, b => b.AppendXamlMethodSignature(context.EnclosingMethodDeclaration.Element));
                builder.AppendLine(1, CurlyBracketOpen);
                builder.AppendLine(2, CompletionMarker);
                builder.AppendLine(1, CurlyBracketClose);
            }
            builder.Append(CurlyBracketClose);

            return builder.ToString();
        }

        private static void AppendLine([NotNull] this StringBuilder builder, int indentDepth, string elem)
        {
            for (var i = 0; i < indentDepth; i++)
            {
                builder.Append(Indent);
            }
            builder.Append(elem);
            builder.Append(LineBreak);
        }

        private static void AppendLine([NotNull] this StringBuilder builder, int indentDepth, [NotNull] Action<StringBuilder> elem)
        {
            for (var i = 0; i < indentDepth; i++)
            {
                builder.Append(Indent);
            }
            elem(builder);
            builder.Append(LineBreak);
        }

        private static void AppendHierarchy([NotNull] this StringBuilder builder, [NotNull] ITypeHierarchy hierarchy)
        {
            builder.AppendHierarchyElement(hierarchy.Element);
            if (hierarchy.HasSupertypes)
            {
                builder.AppendSupertypes(hierarchy);
            }
        }

        private static void AppendHierarchyElement([NotNull] this StringBuilder builder, [NotNull] ITypeName typeName)
        {
            builder.Append(BoldStart)
                .Append(typeName.ToTypeCategory())
                .Append(BoldEnd)
                .Append(Space)
                .Append(typeName.FullName);
        }

        private static void AppendSupertypes([NotNull] this StringBuilder builder, [NotNull] ITypeHierarchy hierarchy)
        {
            builder.Append(BoldStart).Append(" : ").Append(BoldEnd);
            var supertypes = new List<ITypeHierarchy>();
            if (hierarchy.HasSuperclass)
            {
                supertypes.Add(hierarchy.Extends);
            }
            supertypes.AddRange(hierarchy.Implements);
            var supertypeNames = supertypes.Select(type => type.Element.FullName);
            builder.Append(string.Join(", ", supertypeNames));
        }

        private static void AppendXamlMethodSignature([NotNull] this StringBuilder builder, [NotNull] IMethodName method)
        {
            builder.Append(method.ReturnType.FullName);
            if (!method.IsConstructor)
            {
                builder.Append(Space).Append(method.Name);
            }
            builder.Append("(");
            if (method.HasParameters)
            {
                var paramList = method.Parameters.Select(param => param.ValueType.FullName + Space + param.Name);
                builder.Append(string.Join(", ", paramList));
            }
            builder.Append(")");
        }
    }
}
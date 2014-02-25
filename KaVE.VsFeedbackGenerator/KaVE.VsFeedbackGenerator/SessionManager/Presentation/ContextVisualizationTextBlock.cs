using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using JetBrains.Annotations;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    internal class ContextVisualizationTextBlock : TextBlock
    {
        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register(
                "Context",
                typeof (Context),
                typeof (ContextVisualizationTextBlock),
                new PropertyMetadata(OnContextChanged));

        public Context Context
        {
            set { SetValue(ContextProperty, value); }
        }

        // TODO ausprobieren ob diese methode aus dem setter aufgerufen werden kann
        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var representation = ToXaml((Context) e.NewValue);

            // TODO move strings to constants
            var generatedDataTemplate =
                (DataTemplate)
                    XamlReader.Parse(
                        "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock xml:space=\"preserve\">" +
                        representation + "</TextBlock></DataTemplate>");
            var generatedTextBlock = (TextBlock) generatedDataTemplate.LoadContent();

            var textBlock = (ContextVisualizationTextBlock) d;
            textBlock.Inlines.Clear();
            textBlock.Inlines.AddRange(generatedTextBlock.Inlines.ToList());
        }

        // TODO text korrigieren (lokalisierung in resource file) oder ggf raus
        internal const string NoContext = "no constant available";
        internal const string LineBreak = "<LineBreak />";
        internal const string Indent = "  ";
        internal const string Space = " ";
        internal const string CurlyBracketOpen = "{";
        internal const string CurlyBracketClose = "}";
        internal const string BoldStart = "<Bold>";
        internal const string BoldEnd = "</Bold>";
        internal const string Completion = "<Italic Foreground=\"Blue\">@Completion</Italic>";

        internal static string ToXaml([NotNull] Context context)
        {
            if (context == null || context.EnclosingClassHierarchy == null)
            {
                return NoContext;
            }

            var builder = new StringBuilder();

            builder.Append(ToXaml(context.EnclosingClassHierarchy)).Append(LineBreak);
            builder.Append(CurlyBracketOpen).Append(LineBreak);
            if (context.EnclosingMethod == null)
            {
                builder.Append(Indent).Append(Completion).Append(LineBreak);
            }
            else
            {
                AppendLine(builder, 1, ToXamlMethodSignature(context.EnclosingMethod));
                builder.Append(Indent).Append(ToXamlMethodSignature(context.EnclosingMethod)).Append(LineBreak);
                builder.Append(Indent).Append(CurlyBracketOpen).Append(LineBreak);
                builder.Append(Indent).Append(Indent).Append(Completion).Append(LineBreak);
                builder.Append(Indent).Append(CurlyBracketClose).Append(LineBreak);
            }
            builder.Append(CurlyBracketClose);

            return builder.ToString();
        }

        private static void AppendLine(StringBuilder builder, int indentDepth, string text)
        {
            // TODO move idea from above to in here!
        }

        private static string ToXaml([NotNull] ITypeHierarchy hierarchy)
        {
            var builder = new StringBuilder();
            AppendHierarchyElement(builder, hierarchy.Element);
            if (HasSupertypes(hierarchy))
            {
                AppendSupertypes(builder, hierarchy);
            }
            return builder.ToString();
        }

        private static void AppendHierarchyElement(StringBuilder builder, ITypeName typeName)
        {
            builder.Append(BoldStart)
                .Append(typeName.ToTypeCategory())
                .Append(BoldEnd)
                .Append(Space)
                .Append(typeName.FullName);
        }

        private static void AppendSupertypes(StringBuilder builder, ITypeHierarchy hierarchy)
        {
            builder.Append(BoldStart).Append(" : ").Append(BoldEnd);
            var supertypes = new List<ITypeHierarchy>();
            if (HasSuperclass(hierarchy))
            {
                supertypes.Add(hierarchy.Extends);
            }
            supertypes.AddRange(hierarchy.Implements);
            var supertypeNames = supertypes.Select(type => type.Element.FullName);
            builder.Append(string.Join(", ", supertypeNames));
        }

        // TODO move 3 helpers below to hierarchy type
        private static bool HasSupertypes(ITypeHierarchy hierarchy)
        {
            return HasSuperclass(hierarchy) || IsImplementingInterfaces(hierarchy);
        }

        private static bool IsImplementingInterfaces(ITypeHierarchy hierarchy)
        {
            return !hierarchy.Implements.IsEmpty();
        }

        private static bool HasSuperclass(ITypeHierarchy hierarchy)
        {
            return hierarchy.Extends != null;
        }

        private static string ToXamlMethodSignature([NotNull] IMethodName method)
        {
            var builder = new StringBuilder();

            builder.Append(method.ReturnType.FullName);
            if (!method.IsConstructor)
            {
                builder.Append(Space).Append(method.Name);
            }
            builder.Append("(");
            if (method.HasParameters)
            {
                // TODO use string.Join instead of loop

                var enumerator = method.Parameters.GetEnumerator();
                enumerator.MoveNext();
                builder.Append(enumerator.Current.ValueType.FullName).Append(Space).Append(enumerator.Current.Name);
                while (enumerator.MoveNext())
                {
                    builder.Append(", ")
                        .Append(enumerator.Current.ValueType.FullName)
                        .Append(Space)
                        .Append(enumerator.Current.Name);
                }
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
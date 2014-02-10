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
            get { return (Context) GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var representation = ToXamlRepresentation((Context) e.NewValue);

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

        internal const string NoContext = "no constant available";
        internal const string LineBreak = "<LineBreak />";
        internal const string Indent = "  ";
        internal const string Space = " ";
        internal const string CurlyBracketOpen = "{";
        internal const string CurlyBracketClose = "}";
        internal const string BoldStart = "<Bold>";
        internal const string BoldEnd = "</Bold>";
        internal const string Completion = "<Italic Foreground=\"Blue\">@Completion</Italic>";

        internal static string ToXamlRepresentation(Context context)
        {
            if (context == null || context.EnclosingClassHierarchy == null ||
                context.EnclosingClassHierarchy.Element == null)
            {
                return NoContext;
            }

            var builder = new StringBuilder();

            builder.Append(ToXamlRepresentation(context.EnclosingClassHierarchy)).Append(LineBreak);
            builder.Append(CurlyBracketOpen).Append(LineBreak);
            if (context.EnclosingMethod == null)
            {
                builder.Append(Indent).Append(Completion).Append(LineBreak);
            }
            else
            {
                builder.Append(Indent).Append(ToXamlRepresentation(context.EnclosingMethod)).Append(LineBreak);
                builder.Append(Indent).Append(CurlyBracketOpen).Append(LineBreak);
                builder.Append(Indent).Append(Indent).Append(Completion).Append(LineBreak);
                builder.Append(Indent).Append(CurlyBracketClose).Append(LineBreak);
            }
            builder.Append(CurlyBracketClose);

            return builder.ToString();
        }

        internal static string ToXamlRepresentation([NotNull] ITypeHierarchy hierarchy)
        {
            Asserts.NotNull(hierarchy.Element, "TypeHierarchy.Element should not be null");
            var builder = new StringBuilder();

            builder.Append(BoldStart)
                .Append(hierarchy.Element.KindOfType())
                .Append(BoldEnd)
                .Append(Space)
                .Append(hierarchy.Element.FullName);
            var extends = hierarchy.Extends != null && hierarchy.Extends.Element != null;
            var implements = !hierarchy.Implements.IsNullOrEmpty() && hierarchy.Implements.Any(i => i.Element != null);
            if (extends || implements)
            {
                builder.Append(BoldStart).Append(" : ").Append(BoldEnd);
                if (extends)
                {
                    builder.Append(hierarchy.Extends.Element.FullName);
                    if (implements)
                    {
                        hierarchy.Implements.Where(i => i.Element != null)
                            .ForEach(i => builder.Append(", ").Append(i.Element.FullName));
                    }
                }
                else
                {
                    var enumerator = hierarchy.Implements.Where(i => i.Element != null).GetEnumerator();
                    enumerator.MoveNext();
                    // there has to be at least one element because of the definition of implements
                    builder.Append(enumerator.Current.Element.FullName);
                    while (enumerator.MoveNext())
                    {
                        builder.Append(", ").Append(enumerator.Current.Element.FullName);
                    }
                }
            }

            return builder.ToString();
        }

        internal static string ToXamlRepresentation([NotNull] IMethodName method)
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
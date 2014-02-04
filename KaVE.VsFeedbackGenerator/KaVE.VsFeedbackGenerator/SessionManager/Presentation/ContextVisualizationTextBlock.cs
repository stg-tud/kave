using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KaVE.Model.Events.CompletionEvent;

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
                        "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock>" +
                        representation + "</TextBlock></DataTemplate>");
            var generatedTextBlock = (TextBlock) generatedDataTemplate.LoadContent();

            ((ContextVisualizationTextBlock) d).Inlines.AddRange(generatedTextBlock.Inlines.ToList());
        }

        internal static string ToXamlRepresentation(Context context)
        {
            return "This is<LineBreak />some <Bold>static</Bold> context <Italic>as</Italic> a <Underline>first</Underline> trial";
        }
    }
}
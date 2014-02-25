using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KaVE.Model.Events.CompletionEvent;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    internal class ContextVisualizationTextBlock : TextBlock
    {
        private const string DataTemplateBegin =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock xml:space=\"preserve\">";

        private const string DataTemplateEnd = "</TextBlock></DataTemplate>";

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

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var representation = ((Context) e.NewValue).ToXaml();
            var generatedDataTemplate =
                (DataTemplate) XamlReader.Parse(DataTemplateBegin + representation + DataTemplateEnd);
            var generatedTextBlock = (TextBlock)generatedDataTemplate.LoadContent();

            var textBlock = (ContextVisualizationTextBlock)d;
            textBlock.Inlines.Clear();
            textBlock.Inlines.AddRange(generatedTextBlock.Inlines.ToList());
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    internal class BindableRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty ContextDocProperty =
            DependencyProperty.Register(
                "ContextDoc",
                typeof (Context),
                typeof (BindableRichTextBox),
                new PropertyMetadata(OnContextDocChanged));

        public Context ContextDoc
        {
            get { return (Context) GetValue(ContextDocProperty); }
            set { SetValue(ContextDocProperty, value); }
        }

        private static void OnContextDocChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BindableRichTextBox) d).Document = ((Context) e.NewValue).ToFlowDocument();
        }
    }
}
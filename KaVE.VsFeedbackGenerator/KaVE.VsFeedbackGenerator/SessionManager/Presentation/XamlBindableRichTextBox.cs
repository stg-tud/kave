using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    internal class XamlBindableRichTextBox : RichTextBox
    {
        private const string DataTemplateBegin =
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock xml:space=\"preserve\">";

        private const string DataTemplateEnd = "</TextBlock></DataTemplate>";

        public XamlBindableRichTextBox()
        {
            IsReadOnly = true;
        }

        public static readonly DependencyProperty XamlProperty =
            DependencyProperty.Register(
                "Xaml",
                typeof (string),
                typeof (XamlBindableRichTextBox),
                new PropertyMetadata(OnXamlChanged));

        public string Xaml
        {
            set { SetValue(XamlProperty, value); }
        }

        private static void OnXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var xaml = (string) e.NewValue;
            // TODO @Dennis: check for other special chars and move this to the xaml converter, since it should produce valid Xaml!
            var escaped = xaml.Replace("&", "&amp;");
            var generatedDataTemplate =
                (DataTemplate) XamlReader.Parse(DataTemplateBegin + escaped + DataTemplateEnd);
            var generatedTextBlock = (TextBlock) generatedDataTemplate.LoadContent();

            var richTextBox = (XamlBindableRichTextBox) d;
            var document = new FlowDocument();
            var para = new Paragraph();
            para.Inlines.AddRange(generatedTextBlock.Inlines.ToList());
            document.Blocks.Add(para);
            richTextBox.Document = document;
        }
    }
}
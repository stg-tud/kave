using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using KaVE.VsFeedbackGenerator.Utils;

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
            var richTextBox = (XamlBindableRichTextBox) d;
            var document = new FlowDocument();

            var xaml = (string) e.NewValue;
            if (xaml != null)
            {
                DataTemplate generatedDataTemplate;
                try
                {
                    generatedDataTemplate =
                        (DataTemplate) XamlReader.Parse(DataTemplateBegin + xaml + DataTemplateEnd);
                }
                catch (Exception exception)
                {
                    var logEventGenerator = Registry.GetComponent<Generators.ILogger>();
                    logEventGenerator.Error(exception, "Parsing Xaml in XamlBindableRichTextBox");
                    throw;
                }
                var generatedTextBlock = (TextBlock) generatedDataTemplate.LoadContent();

                var para = new Paragraph();
                para.Inlines.AddRange(generatedTextBlock.Inlines.ToList());
                document.Blocks.Add(para);
            }
            richTextBox.Document = document;
        }
    }
}
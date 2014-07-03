/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.Text.RegularExpressions;
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

        private static readonly Regex NodeRegex = new Regex("<.*?>", RegexOptions.Compiled);

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
                if (ContainsTooManyNodesForDisplay(xaml))
                {
                    xaml = StripNodes(xaml);
                }

                var para = CreateParagraphFromXaml(xaml);
                document.Blocks.Add(para);
            }
            richTextBox.Document = document;
        }

        private static bool ContainsTooManyNodesForDisplay(string xaml)
        {
            var tags = 0;
            foreach (var c in xaml.Where(c => c == '<'))
            {
                tags++;
                if (tags > (65533*2))
                {
                    return true;
                }
            }
            return false;
        }

        public static string StripNodes(string xaml)
        {
            return NodeRegex.Replace(xaml, string.Empty);
        }

        private static Paragraph CreateParagraphFromXaml(string xaml)
        {
            try
            {
                var template = (DataTemplate)XamlReader.Parse(DataTemplateBegin + xaml + DataTemplateEnd);
                var textBlock = (TextBlock)template.LoadContent();

                var par = new Paragraph();
                par.Inlines.AddRange(textBlock.Inlines.ToList());
                return par;
            }
            catch (Exception exception)
            {
                var logEventGenerator = Registry.GetComponent<Generators.ILogger>();
                logEventGenerator.Error(exception, "Parsing Xaml in XamlBindableRichTextBox");
                throw;
            }
        }
    }
}
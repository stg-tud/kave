using System.Windows;
using System.Windows.Controls;
using KaVE.Model.Events;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    internal class RawViewTextBlock : TextBlock
    {
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register(
                "Event",
                typeof (IDEEvent),
                typeof (RawViewTextBlock),
                new PropertyMetadata(OnEventChanged));

        public IDEEvent Event
        {
            set { SetValue(EventProperty, value); }
        }

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = (RawViewTextBlock) d;
            textBlock.Text = e.NewValue == null ? null : JsonConvert.SerializeObject(e.NewValue, Formatting.Indented);
        }
    }
}
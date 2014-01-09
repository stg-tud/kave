using System;
using System.Globalization;
using System.Windows.Data;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var indicator = value as bool?;
            Asserts.NotNull(indicator, "value is not a bool");
            return !indicator.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var indicator = value as bool?;
            Asserts.NotNull(indicator, "value is not a bool");
            return !indicator.Value;
        }
    }
}
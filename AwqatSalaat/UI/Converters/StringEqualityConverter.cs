using System;
using System.Globalization;
using System.Windows.Data;

namespace AwqatSalaat.UI.Converters
{
    internal class StringEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string comparand && !string.IsNullOrEmpty(comparand))
            {
                return string.Equals(value?.ToString(), comparand, StringComparison.Ordinal);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

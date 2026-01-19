using AwqatSalaat.Configurations;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AwqatSalaat.UI.Converters
{
    internal class ThemeAccentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ThemeAccent accent)
            {
                try
                {
                    var dictionary = ThemeDictionary.GetAccentDictionary(ThemeManager.GeneralTheme, accent.ToString());
                    var color = (Color)dictionary["ThemeColors.AccentColor"];
                    return new SolidColorBrush(color);
                }
                catch
                {
                    return new SolidColorBrush(Colors.Magenta);
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
